package com.example.caffwebshop.activity

import android.content.Intent
import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.util.Log
import android.widget.Toast
import com.example.caffwebshop.R
import com.example.caffwebshop.model.UserAuthenticateModel
import com.example.caffwebshop.model.UserLoginResponse
import com.example.caffwebshop.network.AuthInteractor
import kotlinx.android.synthetic.main.activity_login.*


class LoginActivity : AppCompatActivity() {

    private lateinit var authInteractor: AuthInteractor

    public override fun onCreate(savedInstanceState: Bundle?) {
        authInteractor= AuthInteractor()
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)
        btn_login!!.setOnClickListener { login() }
        link_signup!!.setOnClickListener { // Start the Signup activity
            val intent = Intent(applicationContext, SignupActivity::class.java)
            startActivityForResult(intent, REQUEST_SIGNUP)
        }
    }

    fun login() {
        Log.d(TAG, "Login")

        btn_login!!.isEnabled = true

        val user=UserAuthenticateModel(input_username.text.toString(),input_password.text.toString())
        authInteractor.login(user, this::onLoginSuccess,this::onLoginError)


    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if (requestCode == REQUEST_SIGNUP) {
            if (resultCode == RESULT_OK) {

                finish()
            }
        }
    }

    override fun onBackPressed() {
        // disable going back to the MainActivity
        moveTaskToBack(true)
    }



    fun onLoginSuccess(ret: UserLoginResponse){

        val token ="Bearer "+ret.jwtToken
        val intent = Intent(applicationContext, WebshopActivity::class.java)
        intent.putExtra("token", token)
        startActivity(intent)
    }

    fun onLoginError(e: Throwable){
        input_username.error="Not a valid username or password!"
        input_password.error="Not a valid username or password!"
        e.printStackTrace()
    }



    /*fun validate(): Boolean {
        var valid = true
        val email = input_username!!.text.toString()
        val password = input_password!!.text.toString()
        if (email.isEmpty() || !Patterns.EMAIL_ADDRESS.matcher(email).matches()) {
            input_username!!.error = "enter a valid email address"
            valid = false
        } else {
            input_username!!.error = null
        }
        if (password.isEmpty() || password.length < 4 || password.length > 10) {
            input_password!!.error = "between 4 and 10 alphanumeric characters"
            valid = false
        } else {
            input_password!!.error = null
        }
        return valid
    }*/

    companion object {
        private const val TAG = "LoginActivity"
        private const val REQUEST_SIGNUP = 0
    }
}