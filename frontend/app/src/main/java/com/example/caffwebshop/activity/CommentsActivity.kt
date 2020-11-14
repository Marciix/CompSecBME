package com.example.caffwebshop.activity

import android.content.DialogInterface
import android.os.Bundle
import android.os.Environment
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
import android.util.Log
import okhttp3.ResponseBody
import java.io.*
import java.lang.Exception
import java.io.File.separator


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
            caffInteractor.getCaffItemsByIDDownload(token,id,this::onSaveSuccess,this::onSaveError)
        }

        iv_delete.setOnClickListener {
            val builder = AlertDialog.Builder(this)
            builder.setMessage("Do you really want to delete this file? ")
            builder.setPositiveButton("Yes", DialogInterface.OnClickListener { dialogInterface: DialogInterface, i: Int ->
                caffInteractor.deleteCaffItem(token,id,this::onDeleteSuccess, this::onDeleteError)
            })
            builder.setNegativeButton("NO", null)
            builder.show()


        }


        rv_comments.layoutManager= LinearLayoutManager(applicationContext)


        val url = "https://caffshop-api.nkelemen.hu/caffitems/$id/preview.jpg"
        val glideUrl = GlideUrl(url) { mapOf(Pair("Authorization", token)) }


        Glide.with(applicationContext)
            .load(glideUrl)
            .into(iv_preview)


        caffInteractor.getCaffItemsByIDComment(token = token, param = id, withAuthors = true, onSuccess = this::onLoadCommentsSuccess, onError = this::onLoadCommentsError)


    }

    private fun onDeleteSuccess(v: Void?){
        Toast.makeText(applicationContext,"File successfuly deleted!", Toast.LENGTH_LONG).show()
        finish()
    }

    private fun onDeleteError(e: Throwable){
        e.printStackTrace()
        Toast.makeText(applicationContext,"You are not able to delete this file!", Toast.LENGTH_LONG).show()
    }

    private fun onBuySuccess(v: Void?){
        Toast.makeText(applicationContext,"Successful purchase!", Toast.LENGTH_LONG).show()
    }

    private fun onBuyError(e: Throwable){
        e.printStackTrace()
        var msg= "Unsuccessful purchase!"
        if(e.message.equals("409")) msg="User already purchased this item."
        Toast.makeText(applicationContext,msg , Toast.LENGTH_LONG).show()
    }

    private fun onSaveSuccess(res: ResponseBody?){
        if(res==null){
            onSaveError(Exception())
        }
        else {
            val succ=writeResponseBodyToDisk(res)
            if(!succ)onSaveError(Exception())
        }
    }

    private fun onSaveError(e: Throwable){
        e.printStackTrace()
        Toast.makeText(applicationContext,"Unable to download this file!", Toast.LENGTH_LONG).show()
    }

    private fun writeResponseBodyToDisk(body: ResponseBody): Boolean {
        try {
            val fileToSave = File(getExternalFilesDir(null)?.toString() + separator + "$id.caff")
            var inputStream: InputStream? = null
            var outputStream: OutputStream? = null
            try {
                val fileReader = ByteArray(4096)
                val fileSize = body.contentLength()
                var fileSizeDownloaded: Long = 0
                inputStream = body.byteStream()
                outputStream = FileOutputStream(fileToSave)
                while (true) {
                    val read = inputStream!!.read(fileReader)
                    if (read == -1) {
                        break
                    }
                    outputStream!!.write(fileReader, 0, read)
                    fileSizeDownloaded += read.toLong()
                    Log.d("filedownload", "file download: $fileSizeDownloaded of $fileSize")
                }
                outputStream!!.flush()
                Toast.makeText(applicationContext,"Successful download! Downloaded file: ${fileToSave.absolutePath}", Toast.LENGTH_LONG).show()
                return true
            } catch (e: IOException) {
                return false
            } finally {
                if (inputStream != null) {
                    inputStream!!.close()
                }
                if (outputStream != null) {
                    outputStream!!.close()
                }
            }
        } catch (e: IOException) {
            return false
        }

    }




    private fun onLoadCommentsSuccess(list: List<CommentPublic>?){
        if(list==null) onLoadCommentsError(Exception("Error: loading comments!"))
       else{
            val l= list as MutableList<CommentPublic>
            l.reverse()
            adapter = CommentsAdapter(applicationContext, l, token)
            rv_comments.adapter = adapter
        }
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
        else caffInteractor.getCaffItemsByIDComment(token = token, param = id, withAuthors = true, onSuccess = this::onLoadCommentsSuccess, onError = this::onLoadCommentsError)

    }
    private fun onCommentError(e: Throwable){
        Toast.makeText(applicationContext, "Unable to add comments!", Toast.LENGTH_LONG).show()
        e.printStackTrace()
    }

    override fun getToken()=token

    override fun getId()=id


}
