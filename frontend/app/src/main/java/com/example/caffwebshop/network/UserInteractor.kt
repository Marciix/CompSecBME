package com.example.caffwebshop.network

import android.os.Handler
import android.util.Log
import com.example.caffwebshop.model.UserModifyModel
import com.example.caffwebshop.model.UserPublic
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
        onSuccess: (T?) -> Unit,
        onError: (Throwable) -> Unit
    ) {
        val handler = Handler()
        Thread {
            try {
                Log.i("callrequest", call.request().toString())
                val c=call.execute()
                Log.i("statuscode" ,c.code().toString())
                if(c.code()!=200){
                    Log.d("message", c.message())
                    throw Exception(c.code().toString())
                }
                val response =c.body()
                handler.post {
                    onSuccess(response)
                }

            } catch (e: Exception) {
                e.printStackTrace()
                handler.post { onError(e) }
            }
        }.start()
    }

    fun getUsers(token: String, onSuccess: (List<UserPublic>?) -> Unit, onError: (Throwable) -> Unit){
        val getRequest=userApi.getUsers(token)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)
    }

    fun delete(token :String, param: Int, onSuccess: (Void?) -> Unit, onError: (Throwable) -> Unit){
        val addRequest=userApi.deleteUser(token, param)
        runCallOnBackgroundThread(addRequest,onSuccess, onError)
    }

    fun modify(token: String, id: Int, param: UserModifyModel, onSuccess: (Void?) -> Unit, onError: (Throwable) -> Unit){
        val deleteRequest=userApi.modifyUser(token, id, param)
        runCallOnBackgroundThread(deleteRequest,onSuccess, onError)
    }
}