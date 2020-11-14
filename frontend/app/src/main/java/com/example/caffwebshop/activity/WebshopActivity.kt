package com.example.caffwebshop.activity

import android.content.Intent
import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.widget.GridLayoutManager
import android.widget.Toast
import com.example.caffwebshop.R
import com.example.caffwebshop.adapter.ImagesAdapter
import com.example.caffwebshop.fragment.CommentDialogFragment
import com.example.caffwebshop.fragment.SearchDialogFragment
import com.example.caffwebshop.model.CaffItemPublic
import com.example.caffwebshop.network.CAFFInteractor

import kotlinx.android.synthetic.main.activity_webshop.*
import java.lang.Exception

class WebshopActivity : AppCompatActivity(), SearchDialogFragment.SearchListener {


    private var adapter: ImagesAdapter? = null
    private lateinit var token : String
    private lateinit var role : String
    private var caffInteractor=CAFFInteractor()
    private lateinit var listOfCaffs: MutableList<CaffItemPublic>

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_webshop)
        title=getString(R.string.Webshop)

        token=intent.getStringExtra("token")
        role=intent.getStringExtra("role")

        rvImages.layoutManager = GridLayoutManager( this, 2)
        srlImages.setOnRefreshListener { loadImages() }

        fab_upload.setOnClickListener {
            val intent = Intent(applicationContext, UploadActivity::class.java)
            intent.putExtra("token", token)
            intent.putExtra("role", role)
            startActivity(intent)
        }

        fab_search.setOnClickListener{
            val searchDialogFragment = SearchDialogFragment()
            searchDialogFragment.show(supportFragmentManager, "TAG")
        }
    }

    override fun onResume() {
        super.onResume()
        loadImages()
    }

    private fun loadImages(){
        caffInteractor.getCaffItems(token= token, onSuccess = this::onLoadSucces, onError = this::onLoadError)
    }

    private fun onLoadSucces(list: List<CaffItemPublic>?){
        if(list==null){
            onLoadError(Exception("List is null!"))
        }
        else{
            listOfCaffs=list as MutableList<CaffItemPublic>
            adapter = ImagesAdapter(applicationContext, listOfCaffs, token, role)
            rvImages.adapter = adapter
            srlImages.isRefreshing = false
        }
    }

    private fun onLoadError(e:Throwable){
        Toast.makeText(applicationContext,"Unable to load images!", Toast.LENGTH_LONG).show()
        e.printStackTrace()
    }

    override fun search(searchParam: String) {
        if(searchParam==""){
            loadImages()
        }
        else caffInteractor.getCaffItemsSearch(token= token, param= searchParam, onSuccess = this::onSearchSuccess, onError = this::onSearchError)
    }

    private fun onSearchSuccess(list :List<CaffItemPublic>?){
        if(list==null){
            onLoadError(Exception("List is null!"))
        }
        else{
            listOfCaffs=list as MutableList<CaffItemPublic>
            adapter = ImagesAdapter(applicationContext, listOfCaffs, token, role)
            rvImages.adapter = adapter
            srlImages.isRefreshing = false
        }

    }
    private fun onSearchError(e:Throwable){
        Toast.makeText(applicationContext,"Unable to search this tag!", Toast.LENGTH_LONG).show()
        e.printStackTrace()
    }




}
