package com.example.caffwebshop.activity

import android.content.DialogInterface
import android.os.Bundle
import android.support.design.widget.Snackbar
import android.support.v7.app.AppCompatActivity
import android.support.v7.widget.LinearLayoutManager
import android.view.Menu
import android.view.MenuItem
import android.widget.Toast
import com.bumptech.glide.Glide
import com.bumptech.glide.load.model.GlideUrl
import com.example.caffwebshop.adapter.CommentsAdapter
import com.example.caffwebshop.fragment.CommentDialogFragment
import com.example.caffwebshop.fragment.DescriptionDialogFragment
import com.example.caffwebshop.model.CommentCreationModel
import com.example.caffwebshop.model.CommentPublic
import com.example.caffwebshop.model.IdResult
import com.example.caffwebshop.network.CAFFInteractor
import kotlinx.android.synthetic.main.activity_comments.*
import kotlinx.android.synthetic.main.content_comments.*
import android.support.v7.app.AlertDialog
import android.support.v7.widget.RecyclerView
import okhttp3.ResponseBody
import java.lang.Exception


class CommentsActivity : AppCompatActivity(),CommentDialogFragment.CommentCreationListener,
        DescriptionDialogFragment.DescriptionDataListener {

    private lateinit var token: String
    private lateinit var role: String
    private var id=0
    private val caffInteractor= CAFFInteractor()

    private var adapter: CommentsAdapter? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(com.example.caffwebshop.R.layout.activity_comments)
        token=intent.getStringExtra("token")
        role=intent.getStringExtra("role")
        id=intent.getIntExtra("id", 0)

       title="Comments"
        setSupportActionBar(toolbar)


        iv_comment.setOnClickListener {
            val commentDialogFragment = CommentDialogFragment()
            commentDialogFragment.show(supportFragmentManager, "TAG")
        }

        iv_details.setOnClickListener {
            val descriptionDialogFragment = DescriptionDialogFragment()
            descriptionDialogFragment.show(supportFragmentManager, "TAG")
        }

        iv_buy.setOnClickListener {
            val builder = AlertDialog.Builder(this)
            builder.setMessage("Do you really want to buy it?")
                builder.setPositiveButton("Yes", DialogInterface.OnClickListener { dialogInterface: DialogInterface, i: Int ->
                    caffInteractor.buyCaffItem(token, id, this::onBuySuccess, this::onBuyError)
                })
            builder.setNegativeButton("NO", null)
            builder.show()

        }

        iv_save.setOnClickListener {
            //TODO: call download
        }

        iv_delete.setOnClickListener {
            //TODO: call delete
            if(role!="admin"){
                Toast.makeText(applicationContext,"Delete function is not available for you!", Toast.LENGTH_LONG).show()
            }
        }


        rv_comments.layoutManager= LinearLayoutManager(applicationContext)


        val url = "https://caffshop-api.nkelemen.hu/caffitems/$id/preview.jpg"
        val glideUrl = GlideUrl(url) { mapOf(Pair("Authorization", token)) }


        Glide.with(applicationContext)
            .load(glideUrl)
            .into(iv_preview)


        //withauthors added
        caffInteractor.getCaffItemsByIDComment(token = token, param = id, withAuthors = true, onSuccess = this::onLoadCommentsSuccess, onError = this::onLoadCommentsError)


    }


    private fun onBuySuccess(v: Void?){
        Toast.makeText(applicationContext,"Successful purchase!", Toast.LENGTH_LONG).show()
    }

    private fun onBuyError(e: Throwable){
        e.printStackTrace()
        Toast.makeText(applicationContext,"Unsuccesful purchase!", Toast.LENGTH_LONG).show()
    }




    private fun onLoadCommentsSuccess(list: List<CommentPublic>?){
        if(list==null) onLoadCommentsError(Exception("Error: loading comments!"))
        adapter = CommentsAdapter(applicationContext, list as MutableList<CommentPublic>, token)
        rv_comments.adapter = adapter
    }

    private fun onLoadCommentsError(e: Throwable){
        e.printStackTrace()
        Toast.makeText(applicationContext, "Unable to load comments!", Toast.LENGTH_LONG).show()
    }


    override fun onCommentCreated(comment: String) {
        val c=CommentCreationModel(comment)
        caffInteractor.commentCaffItem(token, id, c, this::onCommentSuccess, this::onCommentError)
    }

    private fun onCommentSuccess(res: IdResult?){
        if(res==null) onCommentError(Exception("Error: write comment"))
        caffInteractor.getCaffItemsByIDComment(token = token, param = id, withAuthors = true, onSuccess = this::onLoadCommentsSuccess, onError = this::onLoadCommentsError)

    }
    private fun onCommentError(e: Throwable){
        Toast.makeText(applicationContext, "Unable to add comments!", Toast.LENGTH_LONG).show()
        e.printStackTrace()
    }

    override fun getToken()=token

    override fun getId()=id


}
