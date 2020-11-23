using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CaffShop.Entities;
using CaffShop.Helpers;
using CaffShop.Interfaces;
using CaffShop.Models.CaffItem;
using CaffShop.Models.Exceptions;
using CaffShop.Models.Options;
using Ganss.XSS;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CaffShop.Services
{
    public class CaffUploadService: ICaffUploadService
    {
        private readonly ICaffParserWrapper _caffParserWrapper;
        private readonly ICaffItemService _caffItemService;
        private readonly ILogger<ICaffUploadService> _logger;

        private readonly string _randomName;
        private readonly string _tempFilePath;
        private readonly string _caffFilePath;
        private readonly string _prevFilePath;
        private readonly string _jsonFilePath;
        private readonly string _finalPreviewPath;

        public CaffUploadService(
            IOptions<UploadOptions> opt,
            ICaffParserWrapper caffParserWrapper,
            ICaffItemService caffItemService,
            ILogger<ICaffUploadService> logger
        )
        {
            var options = opt.Value;
            _caffParserWrapper = caffParserWrapper;
            _caffItemService = caffItemService;
            _logger = logger;

            _randomName = RandomFileNameWithTimestamp();
            _tempFilePath = Path.Combine(options.TempDirPath, _randomName + ".caff");
            _caffFilePath = Path.Combine(options.CaffDirPath, _randomName + ".caff");
            _prevFilePath = Path.Combine(options.PrevDirPath, _randomName + ".ppm");
            _jsonFilePath = Path.Combine(options.TempDirPath, _randomName + ".json");
            _finalPreviewPath = Path.Combine(options.PrevDirPath, _randomName + UploadOptions.PreviewExtension);
        }


        public async Task<CaffItem> UploadCaffFile(IFormFile file, string originalName, long userId)
        {
            try
            {
                await CopyFormFileToTemp(file);
                ParseCaffFile();
                ConvertPpmToJpg();
                var caffMeta = ReadCaffMetaJson();
                caffMeta = SanitizeCaffItemUploadMeta(caffMeta);
                var caffItem = CreateCaffItem(originalName, userId, caffMeta);
                return await _caffItemService.SaveCaff(caffItem);
            }
            catch (InvalidCaffFileException ex)
            {
                DeleteFiles();
                throw new CaffUploadException(ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex.Message);
                DeleteFiles();
                throw new CaffUploadException("Failed to properly process Caff file.");
            }
            catch (MagickException ex)
            {
                _logger.LogError(ex.Message);
                DeleteFiles();
                throw new CaffUploadException("Failed to create preview image.");
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogError(ex.Message);
                DeleteFiles();
                throw new CaffUploadException("Failed to parse Caff metadata.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                DeleteFiles();
                throw new CaffUploadException("Error when uploading a Caff file.");
            }
        }

        private void DeleteFiles()
        {
            File.Delete(_tempFilePath);
            File.Delete(_caffFilePath);
            File.Delete(_prevFilePath);
            File.Delete(_jsonFilePath);
            File.Delete(_finalPreviewPath);
        }

        private async Task CopyFormFileToTemp(IFormFile file)
        {
            await using var stream = new FileStream(_tempFilePath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        private void ParseCaffFile()
        {
            _caffParserWrapper.ValidateAndParseCaff(_tempFilePath, _prevFilePath, _jsonFilePath);
            File.Move(_tempFilePath, _caffFilePath);
        }

        private void ConvertPpmToJpg()
        {
            if (false == File.Exists(_prevFilePath))
                throw new FileNotFoundException();

            using var img = new MagickImage(_prevFilePath);
            img.Write(_finalPreviewPath);

            File.Delete(_prevFilePath);
        }

        private CaffItem CreateCaffItem(string originalName, long userId, CaffItemUploadMeta meta)
        {

            return new CaffItem
            {
                Title = meta.Title,
                Tags = meta.Tags,
                OwnerId = userId,
                UploadedAt = DateTime.Now,
                CaffPath = _caffFilePath,
                PreviewPath = _finalPreviewPath,
                InnerName = _randomName,
                OriginalName = originalName
            };
        }

        private CaffItemUploadMeta ReadCaffMetaJson()
        {
            if (!File.Exists(_jsonFilePath))
                throw new FileNotFoundException(_jsonFilePath);

            using var r = new StreamReader(_jsonFilePath);
            var json = r.ReadToEnd();
            var item = JsonConvert.DeserializeObject<CaffItemUploadMeta>(json);
            r.Close();

            File.Delete(_jsonFilePath);

            return item;
        }

        private CaffItemUploadMeta SanitizeCaffItemUploadMeta(CaffItemUploadMeta meta)
        {
            var sanitizer = new HtmlSanitizer();
            meta.Title = sanitizer.Sanitize(meta.Title);
            meta.Tags = meta.Tags.Select(tag => sanitizer.Sanitize(tag)).ToList();
            return meta;
        }

        private static string RandomFileNameWithTimestamp()
        {
            var ts = HelperFunctions.GetUnixTimestamp();
            var rnd = HelperFunctions.GenerateRandomString(10);
            return $"{ts}_{rnd}";
        }
    }
}