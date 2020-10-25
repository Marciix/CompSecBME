package com.example.caffwebshop.activity

import android.os.Bundle
import android.os.Handler
import android.support.v7.app.AppCompatActivity
import android.util.Log
import android.widget.Toast
import com.example.caffwebshop.R
import kotlinx.android.synthetic.main.activity_login.*
import kotlinx.android.synthetic.main.activity_signup.*


class SignupActivity : AppCompatActivity() {



    public override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_signup)
        btn_signup!!.setOnClickListener { signup() }
        link_login!!.setOnClickListener { // Finish the registration screen and return to the Login activity
            finish()
        }
    }

    fun signup() {
        Log.d(TAG, "Signup")
        if (!validate()) {
            onSignupFailed()
            return
        }
        btn_signup!!.isEnabled = false


        val name = et_username!!.text.toString()
        val password = et_password!!.text.toString()

        // TODO: Implement signup logic here.
        Handler().postDelayed(
            { // On complete call either onSignupSuccess or onSignupFailed
                // depending on success
                onSignupSuccess()
                // onSignupFailed()
            }, 3000
        )
    }

    fun onSignupSuccess() {
        btn_signup!!.isEnabled = true
        setResult(RESULT_OK, null)
        finish()
    }

    fun onSignupFailed() {
        Toast.makeText(baseContext, "Login failed", Toast.LENGTH_LONG).show()
        btn_signup!!.isEnabled = true
    }

    fun validate(): Boolean {
        var valid = true
        val name = et_username!!.text.toString()
        val password = et_password!!.text.toString()
        if (name.isEmpty() || name.length < 3) {
            et_username!!.error = "at least 3 characters"
            valid = false
        } else {
            et_username!!.error = null
        }
        if (password.isEmpty() || password.length < 4 || password.length > 10) {
            et_password!!.error = "between 4 and 10 alphanumeric characters"
            valid = false
        } else {
            et_password!!.error = null
        }
        return valid
    }

    companion object {
        private const val TAG = "SignupActivity"
    }
}