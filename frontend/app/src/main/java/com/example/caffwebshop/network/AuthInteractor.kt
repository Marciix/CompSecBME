package com.example.caffwebshop.network

import android.os.Handler
import android.util.Log
import com.example.caffwebshop.model.IdResult
import com.example.caffwebshop.model.UserAuthenticateModel
import com.example.caffwebshop.model.UserLoginResponse
import com.example.caffwebshop.model.UserRegistrationModel
import retrofit2.Call
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory



class AuthInteractor {
    private val authApi: AuthApi

    init {
        val retrofit= Retrofit.Builder()
            .baseUrl(AuthApi.ENDPOINT_URL)
            .addConverterFactory(GsonConverterFactory.create())
            .build()

        this.authApi=retrofit.create(AuthApi::class.java)
    }

    private fun <T> runCallOnBackgroundThread(
        call: Call<T>,
        onSuccess: (T) -> Unit,
        onError: (Throwable) -> Unit
    ) {
        val handler = Handler()
        Thread {
            try {

                val response = call.execute().body()!!

                //Log.d("code", call.execute().code().toString())
                handler.post {
                    onSuccess(response)
                }

            } catch (e: Exception) {
                e.printStackTrace()
                handler.post { onError(e) }
            }
        }.start()
    }

    fun login(param: UserAuthenticateModel, onSuccess: (UserLoginResponse) -> Unit, onError: (Throwable) -> Unit){

        val loginRequest=authApi.login(param)
        runCallOnBackgroundThread(loginRequest,onSuccess, onError)
    }

    fun register(param: UserRegistrationModel, onSuccess: (IdResult) -> Unit, onError: (Throwable) -> Unit){

        val registerRequest=authApi.register(param)
        runCallOnBackgroundThread(registerRequest,onSuccess, onError)

       // Log.d("regParam: ", Gson().toJson(param))
    }

}