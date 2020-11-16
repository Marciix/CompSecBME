package com.example.caffwebshop.activity

import android.Manifest
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.util.Log
import com.example.caffwebshop.model.CaffItemCreation
import com.example.caffwebshop.model.IdResult
import com.example.caffwebshop.network.CAFFInteractor
import com.livinglifetechway.quickpermissions.annotations.OnPermissionsDenied
import com.livinglifetechway.quickpermissions.annotations.WithPermissions
import com.livinglifetechway.quickpermissions.util.QuickPermissionsRequest
import kotlinx.android.synthetic.main.activity_upload.*
import java.io.File
import okhttp3.*
import android.support.design.widget.Snackbar
import android.support.v4.provider.DocumentFile
import okhttp3.RequestBody
import okhttp3.MultipartBody
import okhttp3.OkHttpClient
import java.io.FileOutputStream
import java.lang.Exception
import java.util.concurrent.TimeUnit


class UploadActivity : AppCompatActivity() {
    private lateinit var token:String
    private lateinit var role:String
    private lateinit var selectedFile:Uri
    private lateinit var fileToUpload:File
    private val caffInteractor= CAFFInteractor()
    private lateinit var caffItemCreation: CaffItemCreation


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(com.example.caffwebshop.R.layout.activity_upload)
        token=intent.getStringExtra("token")
        role=intent.getStringExtra("role")
        title="Upload"

        btn_choose_file!!.setOnClickListener{
            val intent = Intent().setType("*/*").setAction(Intent.ACTION_GET_CONTENT)
            startActivityForResult(Intent.createChooser(intent, "Select a file"), 420)
        }

        btn_upload_file.setOnClickListener {
            upload()
        }

    }


    @WithPermissions(
        permissions = [Manifest.permission.WRITE_EXTERNAL_STORAGE, Manifest.permission.READ_EXTERNAL_STORAGE]

    )
    private fun upload(){

        val requestFile = RequestBody.create(
            MediaType.parse("application/octet-stream"),
           fileToUpload
        )
        val body = MultipartBody.Part.createFormData( "", fileToUpload.name, requestFile)

        caffInteractor.uploadCaffItem(token = token, file = body, onSucces = this::onUploadSucces, onError = this::onUploadError)

    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
            if(requestCode == 420 && resultCode == RESULT_OK){
                 selectedFile = data?.data!!
                tv_file_path.text = selectedFile.toString()
                //fileToUpload = File(selectedFile.path)
                //tv_file_path.text=fileToUpload.name
                btn_upload_file.isEnabled=true

                val input = contentResolver.openInputStream(selectedFile)
                fileToUpload=File.createTempFile("temp", ".caff")
                val out = FileOutputStream(fileToUpload)
                val buf = ByteArray(1024)
                var len= input.read(buf)
                while (len>0) {
                    out.write(buf, 0, len)
                    len=input.read(buf)

                }
                out.close()
                input!!.close()
                Log.i("file" , fileToUpload.length().toString())


            }
    }


    private fun onUploadSucces(res:IdResult?){
        Snackbar.make(fragment_item_edit_content,"Image successfully uploaded with id: ${res?.id}", Snackbar.LENGTH_LONG).show()
    }

    private fun onUploadError(e:Throwable){
        Snackbar.make(fragment_item_edit_content,"Unable to upload this image!", Snackbar.LENGTH_LONG).show()
    }

    @OnPermissionsDenied
    fun onDenied(arg: QuickPermissionsRequest) {
        Snackbar.make(fragment_item_edit_content,"Permission denied!", Snackbar.LENGTH_LONG).show()
    }




}
