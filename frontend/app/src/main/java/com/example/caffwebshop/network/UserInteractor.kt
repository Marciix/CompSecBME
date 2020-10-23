package com.example.caffwebshop.network

import android.os.Handler
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

    fun addUser(token :String, param: Int, onSuccess: (Void) -> Unit, onError: (Throwable) -> Unit){

        val addRequest=userApi.addUser(token, param)
        runCallOnBackgroundThread(addRequest,onSuccess, onError)
    }

    fun delete(token: String, param: Int, onSuccess: (Void) -> Unit, onError: (Throwable) -> Unit){

        val deleteRequest=userApi.deleteUser(token, param)
        runCallOnBackgroundThread(deleteRequest,onSuccess, onError)
    }
}