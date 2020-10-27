package com.example.caffwebshop.activity

import android.os.Bundle
import android.os.Handler
import android.support.constraint.motion.TransitionBuilder.validate
import android.support.v7.app.AppCompatActivity
import android.util.Log
import android.util.Patterns
import android.widget.Toast
import com.example.caffwebshop.R
import com.example.caffwebshop.model.IdResult
import com.example.caffwebshop.model.UserRegistrationModel
import com.example.caffwebshop.network.AuthInteractor
import kotlinx.android.synthetic.main.activity_login.*
import kotlinx.android.synthetic.main.activity_signup.*


class SignupActivity : AppCompatActivity() {


    private val authInteractor=AuthInteractor()

    public override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_signup)
        btn_signup!!.setOnClickListener { signup() }
        link_login!!.setOnClickListener { // Finish the registration screen and return to the Login activity
            finish()
        }
    }

    fun signup() {
        if(validate()){
            val user =UserRegistrationModel(et_username.text.toString(),et_email.text.toString(), et_firstname.text.toString(),et_lastname.text.toString(),et_password.text.toString())
            authInteractor.register(user,this::onSignupSuccess, this::onSignupError)
        }
    }

    fun onSignupSuccess(res: IdResult) {
        Toast.makeText(baseContext, "Signup succesful with id: $res.id", Toast.LENGTH_LONG).show()
        finish()
    }

    fun onSignupError(e:Throwable) {
        Toast.makeText(baseContext, "Signup failed", Toast.LENGTH_LONG).show()
        e.printStackTrace()
        //btn_signup!!.isEnabled = true
    }

    fun validate(): Boolean {
        var valid = true
        if (et_email.text.toString().isEmpty() || !Patterns.EMAIL_ADDRESS.matcher(et_email.text.toString()).matches()) {
            et_email!!.error = "Please enter a valid email address!"
            valid = false
        } else {
            et_email!!.error = null
        }
        if (et_password.text.toString().isEmpty()) {
            et_password!!.error = "Please enter Password!"
            valid = false
        } else {
            et_password!!.error = null
        }
        if (et_firstname.text.toString().isEmpty()) {
            et_firstname!!.error = "Please enter First Name!"
            valid = false
        } else {
            et_firstname!!.error = null
        }
        if (et_lastname.text.toString().isEmpty()) {
            et_lastname!!.error = "Please enter Last name!"
            valid = false
        } else {
            et_lastname!!.error = null
        }
        if (et_username.text.toString().isEmpty()) {
            et_username!!.error = "Please enter Username!"
            valid = false
        } else {
            et_username!!.error = null
        }
        return valid
    }



    companion object {
        private const val TAG = "SignupActivity"
    }
}