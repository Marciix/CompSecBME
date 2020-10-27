package com.example.caffwebshop.activity

import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.widget.LinearLayoutManager
import android.widget.Toast
import com.bumptech.glide.Glide
import com.bumptech.glide.load.model.GlideUrl
import com.example.caffwebshop.adapter.CommentsAdapter
import com.example.caffwebshop.model.CommentPublic

import kotlinx.android.synthetic.main.activity_comments.*
import kotlinx.android.synthetic.main.content_comments.*
import com.example.caffwebshop.R
import com.example.caffwebshop.network.CAFFInteractor


class CommentsActivity : AppCompatActivity() {
    private lateinit var token: String
    private var id=0
    private val caffInteractor= CAFFInteractor()

    private var adapter: CommentsAdapter? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_comments)
        setSupportActionBar(toolbar)

        token=intent.getStringExtra("token")
        id=intent.getIntExtra("id", 0)
        rv_comments.layoutManager=LinearLayoutManager(applicationContext)

        val url = "https://caffshop-api.nkelemen.hu/caffitems/$id/preview.jpg"
        val glideUrl = GlideUrl(url) { mapOf(Pair("Authorization", token)) }


        Glide.with(applicationContext)
            .load(glideUrl)
            .into(iv_preview)

        caffInteractor.getCaffItemsByIDComment(token=token, param=id,onSuccess = this::onLoadCommentsSuccess, onError = this::onLoadCommentsError)


    }

    private fun onLoadCommentsSuccess(list: List<CommentPublic>){
        adapter = CommentsAdapter(applicationContext, list as MutableList<CommentPublic>, token)
        rv_comments.adapter = adapter
    }

    private fun onLoadCommentsError(e:Throwable){
        e.printStackTrace()
        Toast.makeText(applicationContext, "Unable to load comments!", Toast.LENGTH_LONG).show()
    }

}
