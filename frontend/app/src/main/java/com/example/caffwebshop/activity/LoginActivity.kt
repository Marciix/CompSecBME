
package com.example.caffwebshop.activity


import android.content.Intent
import android.os.Bundle
import android.os.Handler
import android.support.v7.app.AppCompatActivity
import android.util.Log
import android.util.Patterns
import android.widget.Toast
import com.example.caffwebshop.R
import kotlinx.android.synthetic.main.activity_login.*


class LoginActivity : AppCompatActivity() {





    public override fun onCreate(savedInstanceState: Bundle?) {
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
        if (!validate()) {
            onLoginFailed()
            return
        }
        btn_login!!.isEnabled = false

        val email = input_username!!.text.toString()
        val password = input_password!!.text.toString()

        // TODO: Implement your own authentication logic here.
        Handler().postDelayed(
            { // On complete call either onLoginSuccess or onLoginFailed
                onLoginSuccess()
                // onLoginFailed();

            }, 3000
        )
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if (requestCode == REQUEST_SIGNUP) {
            if (resultCode == RESULT_OK) {

                // TODO: Implement successful signup logic here
                // By default we just finish the Activity and log them in automatically
                finish()
            }
        }
    }

    override fun onBackPressed() {
        // disable going back to the MainActivity
        moveTaskToBack(true)
    }

    fun onLoginSuccess() {
        btn_login!!.isEnabled = true
        // val intent = Intent(applicationContext, MainActivity::class.java)
        //startActivityForResult(intent, REQUEST_SIGNUP)
        finish()


    }

    fun onLoginFailed() {
        Toast.makeText(baseContext, "Login failed", Toast.LENGTH_LONG).show()
        btn_login!!.isEnabled = true
    }

    fun validate(): Boolean {
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
    }

    companion object {
        private const val TAG = "LoginActivity"
        private const val REQUEST_SIGNUP = 0
    }
}