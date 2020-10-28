package com.example.caffwebshop.fragment

import android.app.Activity
import android.app.Dialog
import android.os.Bundle
import android.support.v4.app.DialogFragment
import android.support.v7.app.AlertDialog
import android.view.LayoutInflater
import android.view.View
import android.widget.EditText
import com.example.caffwebshop.R

class CommentDialogFragment: DialogFragment(){

    private lateinit var listener:CommentCreationListener
    private lateinit var a:Activity
    private lateinit var etComment: EditText
    private lateinit var viewAct: View

    interface CommentCreationListener{
        fun onCommentCreated(comment: String)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        val act=this.activity
        a=this.activity as Activity

        if(act is CommentCreationListener){
            listener=act
        }
        else{
            throw RuntimeException("Activity must implement SearchListener")
        }

    }

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {

        val builder = AlertDialog.Builder(activity!!)

        builder.setView(getContentView())
        builder.setTitle("Add new comment")
        builder.setPositiveButton("Post") { _, _ ->
          listener.onCommentCreated(etComment.text.toString())
        }
        builder.setNegativeButton("Cancel", null)
        return builder.create()

    }

    private fun getContentView(): View {
        viewAct = LayoutInflater.from(context).inflate(R.layout.comment_fragment, null)
        etComment=viewAct.findViewById(R.id.et_newcomment)
        return viewAct
    }
}