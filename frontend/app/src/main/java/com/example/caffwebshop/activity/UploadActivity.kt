package com.example.caffwebshop.activity

import android.Manifest
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.util.Log
import android.widget.Toast
import com.example.caffwebshop.model.CaffItemCreation
import com.example.caffwebshop.model.IdResult
import com.example.caffwebshop.network.CAFFInteractor
import com.livinglifetechway.quickpermissions.annotations.OnPermissionsDenied
import com.livinglifetechway.quickpermissions.annotations.WithPermissions
import com.livinglifetechway.quickpermissions.util.QuickPermissionsRequest
import kotlinx.android.synthetic.main.activity_upload.*
import java.io.File
import android.os.AsyncTask.execute
import android.os.Environment
import okhttp3.*
import java.net.URI
import android.os.Environment.getExternalStorageDirectory




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
                tv_file_path.text = selectedFile.path
                fileToUpload = File(selectedFile.path)
                btn_upload_file.isEnabled=true
            }
    }


    private fun onUploadSucces(res:IdResult?){
        Toast.makeText(applicationContext,"Successful upload with id: ${res?.id}!", Toast.LENGTH_LONG).show()
    }

    private fun onUploadError(e:Throwable){
        Toast.makeText(applicationContext,"Unable to upload images!", Toast.LENGTH_LONG).show()
        e.printStackTrace()
    }

    @OnPermissionsDenied
    fun onDenied(arg: QuickPermissionsRequest) {
        Toast.makeText(applicationContext,"Permission denied!", Toast.LENGTH_LONG).show()
    }
}
