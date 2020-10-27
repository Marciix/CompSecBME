package com.example.caffwebshop.activity

import android.content.Intent
import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.widget.GridLayoutManager
import android.widget.Toast
import com.example.caffwebshop.R
import com.example.caffwebshop.adapter.ImagesAdapter
import com.example.caffwebshop.model.CaffItemPublic
import com.example.caffwebshop.network.CAFFInteractor

import kotlinx.android.synthetic.main.activity_webshop.*

class WebshopActivity : AppCompatActivity() {

    private var adapter: ImagesAdapter? = null
    private lateinit var token : String
    private var caffInteractor=CAFFInteractor()
    private lateinit var listOfCaffs: MutableList<CaffItemPublic>

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_webshop)

        token=intent.getStringExtra("token")

        rvImages.layoutManager = GridLayoutManager( this, 2)
        srlImages.setOnRefreshListener { loadImages() }

        fab_upload.setOnClickListener {
            val intent = Intent(applicationContext, UploadActivity::class.java)
            intent.putExtra("token", token)
            startActivity(intent)
        }
    }

    override fun onResume() {
        super.onResume()
        loadImages()
    }

    private fun loadImages(){
        caffInteractor.getCaffItems(token= token, onSuccess = this::onLoadSucces, onError = this::onLoadError)
    }

    private fun onLoadSucces(list: List<CaffItemPublic>){
        listOfCaffs=list as MutableList<CaffItemPublic>
        val images: MutableList<String> = mutableListOf()
        for(item in listOfCaffs){
            images += "https://images.unsplash.com/photo-1486758206125-94d07f414b1c?ixlib=rb-0.3.5&s=2bda5e189cbdf19185f03f310a88ae5b&auto=format&fit=crop&w=1950&q=80"
        }

        adapter = ImagesAdapter(applicationContext, images, token)
        rvImages.adapter = adapter
        srlImages.isRefreshing = false
    }

    private fun onLoadError(e:Throwable){
        Toast.makeText(applicationContext,"Unable to load images!", Toast.LENGTH_LONG).show()
        e.printStackTrace()
    }



}
