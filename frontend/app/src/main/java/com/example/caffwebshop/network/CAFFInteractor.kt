package com.example.caffwebshop.network

import android.os.Handler
import android.util.Log
import com.example.caffwebshop.model.CaffItemCreation
import com.example.caffwebshop.model.CaffItemPublic
import com.example.caffwebshop.model.CommentCreationModel
import com.example.caffwebshop.model.CommentPublic
import retrofit2.Call
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class CAFFInteractor {
    private val caffApi: CAFFApi

    init {
        val retrofit= Retrofit.Builder()
            .baseUrl(CAFFApi.ENDPOINT_URL)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
        this.caffApi=retrofit.create(CAFFApi::class.java)
    }

    private fun <T> runCallOnBackgroundThread(
        call: Call<T>,
        onSuccess: (T) -> Unit,
        onError: (Throwable) -> Unit
    ) {
        val handler = Handler()
        Thread {
            try {
                //Log.i("status", call.execute().code().toString())
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

    fun getCaffItems(token: String,onSuccess: (List<CaffItemPublic>)->Unit, onError: (Throwable) -> Unit){

        val getRequest=caffApi.getCaffItems(token)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)
        //Log.d("request: ", getRequest.request().toString())

    }

    fun getCaffItemsByID(token: String, param: Int, onSuccess: (CaffItemPublic)->Unit, onError: (Throwable) -> Unit){
        val getRequest=caffApi.getCaffItemsByID(token, param)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    fun getCaffItemsByIDDownload(token: String, param: Int, onSuccess: (Void)->Unit, onError: (Throwable) -> Unit){
        val getRequest=caffApi.getCaffItemsByIDDownload(token, param)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    fun getCaffItemsByIDPreview(token: String, param: Int, onSuccess: (Void)->Unit, onError: (Throwable) -> Unit){
        val getRequest=caffApi.getCaffItemsByIDPreview(token, param)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    fun getCaffItemsSearch(token: String, param: String, onSuccess: (CaffItemPublic)->Unit, onError: (Throwable) -> Unit){
        val getRequest=caffApi.getCaffItemsSearch(token, param)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    fun getCaffItemsByIDComment(token: String, param: Int, onSuccess: (CommentPublic)->Unit, onError: (Throwable) -> Unit){
        val getRequest=caffApi.getCaffItemsByIDComment(token, param)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    fun uploadCaffItem(token: String, param: CaffItemCreation, onSucces: (Int)->Unit, onError: (Throwable) -> Unit){
        val uploadRequest=caffApi.uploadCaffItem(token, param)
        runCallOnBackgroundThread(uploadRequest,onSucces,onError)
    }

    fun buyCaffItem(token: String, param: Int, onSucces: (Void)->Unit, onError: (Throwable) -> Unit){
        val buyRequest=caffApi.buyCaffItem(token, param)
        runCallOnBackgroundThread(buyRequest,onSucces,onError)
    }

    fun commentCaffItem(token: String, id: Int, comment: CommentCreationModel, onSucces: (Int)->Unit, onError: (Throwable) -> Unit){
        val commentRequest=caffApi.commentCaffItem(token, id, comment)
        runCallOnBackgroundThread(commentRequest,onSucces,onError)
    }

    fun deleteCaffItem(token: String, id: Int, onSuccess: (Void) -> Unit, onError: (Throwable) -> Unit){
        val deleteRequest=caffApi.deleteCaffItemByID(token, id)
        runCallOnBackgroundThread(deleteRequest,onSuccess,onError)
    }


}