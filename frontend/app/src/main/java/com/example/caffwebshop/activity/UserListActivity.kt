package com.example.caffwebshop.activity

import android.content.Intent
import android.support.v7.app.AppCompatActivity
import android.os.Bundle
import android.support.design.widget.Snackbar
import android.support.v7.widget.LinearLayoutManager
import com.example.caffwebshop.R
import com.example.caffwebshop.adapter.CommentsAdapter
import com.example.caffwebshop.adapter.UserAdapter
import com.example.caffwebshop.model.CommentPublic
import com.example.caffwebshop.model.UserPublic
import com.example.caffwebshop.network.UserInteractor
import kotlinx.android.synthetic.main.activity_user_list.*
import kotlinx.android.synthetic.main.content_comments.*
import java.lang.Exception

class UserListActivity : AppCompatActivity() {

    private lateinit var token: String
    private lateinit var role: String
    private val userInteractor=UserInteractor()

    private var adapter: UserAdapter? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_user_list)
        title="List of Users"

        token=intent.getStringExtra("token")
        role=intent.getStringExtra("role")

        rv_users.layoutManager= LinearLayoutManager(applicationContext)

        btn_admin.setOnClickListener {
            val intent = Intent(applicationContext, AdminActivity::class.java)
            intent.putExtra("token", token)
            intent.putExtra("role", role)
            startActivity(intent)
        }
        loadUsers()

    }

    private fun loadUsers(){
        userInteractor.getUsers(token, this::onLoadSuccess, this::onLoadError)
    }

    private fun onLoadSuccess(list: List<UserPublic>?){
        if(list==null) onLoadError(Exception("Error!"))
        else{
            val l= list as MutableList<UserPublic>
            l.reverse()
            adapter = UserAdapter(applicationContext, l, token)
            rv_users.adapter = adapter
        }
    }

    private fun onLoadError(e:Throwable){
        e.printStackTrace()
        Snackbar.make(parentUserList,"Unable to load users!", Snackbar.LENGTH_LONG).show()
    }

    override fun onResume() {
        super.onResume()
        loadUsers()
    }
}
