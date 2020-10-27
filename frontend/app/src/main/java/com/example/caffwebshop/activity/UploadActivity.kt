package com.example.caffwebshop.activity

import android.support.v7.app.AppCompatActivity
import android.os.Bundle
import com.example.caffwebshop.R

class UploadActivity : AppCompatActivity() {
    private lateinit var token:String

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_upload)
        token=intent.getStringExtra("token")
    }
}
