package com.example.caffwebshop

import android.content.Intent
import android.os.Bundle
import android.support.v7.app.AppCompatActivity;
import android.util.Log
import android.view.Menu
import android.view.MenuItem
import com.example.caffwebshop.activity.LoginActivity
import com.example.caffwebshop.model.CaffItemPublic
import com.example.caffwebshop.model.UserAuthenticateModel
import com.example.caffwebshop.model.UserLoginResponse
import com.example.caffwebshop.model.UserRegistrationModel
import com.example.caffwebshop.network.AuthInteractor
import com.example.caffwebshop.network.CAFFInteractor

import kotlinx.android.synthetic.main.activity_main.*
import kotlinx.android.synthetic.main.content_main.*

class MainActivity : AppCompatActivity() {
    private var authInteractor=AuthInteractor()
    private var caffInteractor= CAFFInteractor()
    private lateinit var token: String

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        setSupportActionBar(toolbar)

        login.setOnClickListener{
            val intent = Intent(this, LoginActivity::class.java)
            startActivity(intent)
        }

        
        /*register.setOnClickListener { view ->
            val user=UserRegistrationModel("test","b","c", "d", "test")
            authInteractor.register(user, this::onSuccess, this::onError)

        }
        login.setOnClickListener { view ->
            val userAuthenticateModel= UserAuthenticateModel("test", "test")
            authInteractor.login(userAuthenticateModel, this::onSuccessLogin, this::onError)

        }*/
        test.setOnClickListener { view ->

            caffInteractor.getCaffItems(token = token , onSuccess = this::onCaffGetSuccess,onError =  this::onError)

        }
    }

    fun onCaffGetSuccess(c: List<CaffItemPublic>){
        Log.d("succ", c.toString())

    }

    fun onSuccessLogin(i: UserLoginResponse){
        token="Bearer "+i.jwtToken
        val msg= "success id: $token"
        Log.d("succ", msg )
    }

    fun onSuccess(i: Int){
        val msg= "success id: $i"
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
