package com.example.caffwebshop.activity

import android.support.v7.app.AppCompatActivity
import android.os.Bundle
import android.widget.Toast
import com.example.caffwebshop.R
import com.example.caffwebshop.model.UserModifyModel
import com.example.caffwebshop.network.UserInteractor
import kotlinx.android.synthetic.main.activity_admin.*

class AdminActivity : AppCompatActivity() {

    private val userInteractor=UserInteractor()
    private lateinit var token:String
    private lateinit var role:String

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_admin)

        title = "User administration"

        token=intent.getStringExtra("token")
        role=intent.getStringExtra("role")

        btnDeleteUser.setOnClickListener {
            userInteractor.delete(token, Integer.parseInt(etDeleteId.text.toString()), this::onDeleteSuccess, this::onDeleteError)
        }
        btnModifyUser.setOnClickListener {
            val user= UserModifyModel(et_ModifyFirstName.text.toString(), et_ModifyLastName.text.toString())
            userInteractor.modify(token, Integer.parseInt(et_ModifyId.text.toString()),user, this::onModifySuccess, this::onModifyError)
        }
    }

    private fun onDeleteSuccess(v:Void?){
        Toast.makeText(applicationContext, "User successfully deleted!", Toast.LENGTH_LONG).show()
        finish()
    }

    private fun onDeleteError(e:Throwable){
        e.printStackTrace()
        Toast.makeText(applicationContext, "Delete error!", Toast.LENGTH_LONG).show()

    }

    private fun onModifySuccess(v:Void?){
        Toast.makeText(applicationContext, "User successfully modified!", Toast.LENGTH_LONG).show()
        finish()
    }

    private fun onModifyError(e:Throwable){
        e.printStackTrace()
        Toast.makeText(applicationContext, "Modify error!", Toast.LENGTH_LONG).show()

    }



}
