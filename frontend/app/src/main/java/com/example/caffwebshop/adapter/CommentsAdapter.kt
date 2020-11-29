package com.example.caffwebshop.adapter

import android.content.Context
import android.content.Intent
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import com.example.caffwebshop.R
import com.example.caffwebshop.model.CommentPublic
import kotlinx.android.synthetic.main.li_comment.view.*

class CommentsAdapter(
    private val context: Context,
    private val comments: MutableList<CommentPublic>,
    private val token: String)
    : RecyclerView.Adapter<CommentsAdapter.ViewHolder>() {

    private val layoutInflater = LayoutInflater.from(context)

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val view = layoutInflater.inflate(R.layout.li_comment, parent, false)
        return ViewHolder(view)
    }


    override fun getItemCount()= comments.size

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val comment = comments[position]
       if(comment.author!=null){
           holder.user.text=comments[position].author.userName
       }
        else{
           holder.user.text="user"
       }
        holder.date.text=comments[position].createdAt
        holder.comment.text=comments[position].content

    }

    class ViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val user: TextView=view.tv_user
        val date: TextView=view.tv_date
        val comment: TextView=view.tv_comment

    }
}