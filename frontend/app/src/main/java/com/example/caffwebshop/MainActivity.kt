package com.example.caffwebshop

import android.os.Bundle
import android.support.design.widget.Snackbar
import android.support.v7.app.AppCompatActivity;
import android.util.Log
import android.view.Menu
import android.view.MenuItem
import android.widget.Toast
import com.example.caffwebshop.model.UserAuthenticateModel
import com.example.caffwebshop.model.UserLoginResponse
import com.example.caffwebshop.model.UserRegistrationModel
import com.example.caffwebshop.network.AuthInteractor

import kotlinx.android.synthetic.main.activity_main.*

class MainActivity : AppCompatActivity() {
    private var authInteractor=AuthInteractor()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        setSupportActionBar(toolbar)

        fab.setOnClickListener { view ->
            val user=UserRegistrationModel("a","b","c", "d", "e")
            val userAuthenticateModel=UserAuthenticateModel("string", "string")
            //authInteractor.register(user, this::onSuccess, this::onError)
            authInteractor.login(userAuthenticateModel, this::onSuccess, this::onError)


        }
    }

    fun onSuccess(u:UserLoginResponse){
        val msg= "success id:"+u.jwtToken
        Log.d("succ", msg )
    }

    fun onError(e:Throwable){
        e.printStackTrace()
        Log.d("err", "errrorrr")
    }

    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        // Inflate the menu; this adds items to the action bar if it is present.
        menuInflater.inflate(R.menu.menu_main, menu)
        return true
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        return when (item.itemId) {
            R.id.action_settings -> true
            else -> super.onOptionsItemSelected(item)
        }
    }
}
