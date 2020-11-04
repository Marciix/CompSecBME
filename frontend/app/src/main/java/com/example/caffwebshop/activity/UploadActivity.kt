package com.example.caffwebshop.activity

import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.widget.Toast
import com.example.caffwebshop.R
import com.example.caffwebshop.model.CaffItemCreation
import com.example.caffwebshop.network.CAFFInteractor
import kotlinx.android.synthetic.main.activity_upload.*
import java.io.File


class UploadActivity : AppCompatActivity() {
    private lateinit var token:String
    private lateinit var selectedFile:Uri
    private lateinit var fileToUpload:File
    private lateinit var caffInteractor: CAFFInteractor
    private lateinit var caffItemCreation: CaffItemCreation

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_upload)
        token=intent.getStringExtra("token")

        btn_choose_file!!.setOnClickListener{

            val intent = Intent().setType("*/*").setAction(Intent.ACTION_GET_CONTENT)

            startActivityForResult(Intent.createChooser(intent, "Select a file"), 420)

        }

    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
            if(requestCode == 420 && resultCode == RESULT_OK){
                 selectedFile = data?.data!!
                tv_file_path.setText(selectedFile.toString())
                fileToUpload = File(selectedFile.path)
                caffInteractor.uploadCaffItem(token = token, param = caffItemCreation, onSucces = this::onUploadSucces, onError = this::onUploadError)
            }
    }


    private fun onUploadSucces(i:Int){

    }


    private fun onUploadError(e:Throwable){
        Toast.makeText(applicationContext,"Unable to upload images!", Toast.LENGTH_LONG).show()
        e.printStackTrace()
    }

}
