package com.example.caffwebshop

import android.os.Bundle
import android.support.v7.app.AppCompatActivity;
import android.util.Log
import android.view.Menu
import android.view.MenuItem
import com.example.caffwebshop.model.CaffItemPublic
import com.example.caffwebshop.model.UserAuthenticateModel
import com.example.caffwebshop.model.UserLoginResponse
import com.example.caffwebshop.model.UserRegistrationModel
import com.example.caffwebshop.network.AuthInteractor
import com.example.caffwebshop.network.CAFFInteractor

import kotlinx.android.synthetic.main.activity_main.*
import kotlinx.android.synthetic.main.content_main.*
import okhttp3.ResponseBody

class MainActivity : AppCompatActivity() {
    private var authInteractor=AuthInteractor()
    private var caffInteractor= CAFFInteractor()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        setSupportActionBar(toolbar)


        register.setOnClickListener { view ->
            val user=UserRegistrationModel("a","b","c", "d", "e")
            authInteractor.register(user, this::onSuccess, this::onError)

        }
        login.setOnClickListener { view ->
            val userAuthenticateModel= UserAuthenticateModel("string", "string")
            authInteractor.login(userAuthenticateModel, this::onSuccessLogin, this::onError)

        }
        test.setOnClickListener { view ->

            caffInteractor.getCaffItems(this::onCaffGetSuccess, this::onError)

        }
    }

    fun onCaffGetSuccess(c: CaffItemPublic){
        Log.d("succ", "caff get succes" )
    }

    fun onSuccessLogin(i: UserLoginResponse){
        val msg= "success id: ${i.jwtToken}"
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
