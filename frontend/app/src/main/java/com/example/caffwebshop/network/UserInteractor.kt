package com.example.caffwebshop.network

import android.os.Handler
import com.example.caffwebshop.model.UserAuthenticateModel
import com.example.caffwebshop.model.UserLoginResponse
import retrofit2.Call
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class UserInteractor {
    private val userApi: UserApi

    init {
        val retrofit= Retrofit.Builder()
            .baseUrl(UserApi.ENDPOINT_URL)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
        this.userApi=retrofit.create(UserApi::class.java)
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
                handler.post {
                    onSuccess(response)
                }

            } catch (e: Exception) {
                e.printStackTrace()
                handler.post { onError(e) }
            }
        }.start()
    }

    fun addUser(param: Int, onSuccess: (Unit) -> Unit, onError: (Throwable) -> Unit){

        val addRequest=userApi.addUser(param)
        runCallOnBackgroundThread(addRequest,onSuccess, onError)
    }

    fun delete(param: Int, onSuccess: (Unit) -> Unit, onError: (Throwable) -> Unit){

        val deleteRequest=userApi.deleteUser(param)
        runCallOnBackgroundThread(deleteRequest,onSuccess, onError)
    }
}