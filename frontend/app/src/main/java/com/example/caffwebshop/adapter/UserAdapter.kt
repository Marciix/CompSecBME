package com.example.caffwebshop.adapter

import android.content.Context
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import com.example.caffwebshop.R
import com.example.caffwebshop.model.UserPublic
import kotlinx.android.synthetic.main.li_user.view.*

class UserAdapter (private val context: Context,
    private val users: MutableList<UserPublic>,
    private val token: String)
    : RecyclerView.Adapter<UserAdapter.ViewHolder>() {

        private val layoutInflater = LayoutInflater.from(context)

        override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
            val view = layoutInflater.inflate(R.layout.li_user, parent, false)
            return ViewHolder(view)
        }


        override fun getItemCount()= users.size

        override fun onBindViewHolder(holder: ViewHolder, position: Int) {
           val user =users[position]
            holder.email.text="Email: ${user.email}"
            holder.firstname.text="Firstname: ${user.firstName}"
            holder.lastname.text="Lastname: ${user.lastName}"
            holder.userid.text="ID: ${user.id}"
            holder.username.text="Username: ${user.userName}"
        }

        class ViewHolder(view: View) : RecyclerView.ViewHolder(view) {
            val userid: TextView=view.tv_id_user
            val email:TextView=view.tv_email_user
            val username: TextView =view.tv_userName_user
            val firstname: TextView =view.tv_firstName_user
            val lastname: TextView =view.tv_lastName_user

        }
}