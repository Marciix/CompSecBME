package com.example.caffwebshop.network

import android.os.Handler
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

    fun getCaffItems(onSuccess: (CaffItemPublic)->Unit, onError: (Throwable) -> Unit){
        val getRequest=caffApi.getCaffItems()
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    fun getCaffItemsByID(param: Int, onSuccess: (CaffItemPublic)->Unit, onError: (Throwable) -> Unit){
        val getRequest=caffApi.getCaffItemsByID(param)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    fun getCaffItemsByIDDownload(param: Int, onSuccess: (Unit)->Unit, onError: (Throwable) -> Unit){
        val getRequest=caffApi.getCaffItemsByIDDownload(param)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    fun getCaffItemsByIDPreview(param: Int, onSuccess: (Unit)->Unit, onError: (Throwable) -> Unit){
        val getRequest=caffApi.getCaffItemsByIDPreview(param)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    fun getCaffItemsSearch(param: String, onSuccess: (CaffItemPublic)->Unit, onError: (Throwable) -> Unit){
        val getRequest=caffApi.getCaffItemsSearch(param)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    fun getCaffItemsByIDComment(param: Int, onSuccess: (CommentPublic)->Unit, onError: (Throwable) -> Unit){
        val getRequest=caffApi.getCaffItemsByIDComment(param)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    fun uploadCaffItem(param: CaffItemCreation, onSucces: (Int)->Unit, onError: (Throwable) -> Unit){
        val uploadRequest=caffApi.uploadCaffItem(param)
        runCallOnBackgroundThread(uploadRequest,onSucces,onError)
    }

    fun buyCaffItem(param: Int, onSucces: (Unit)->Unit, onError: (Throwable) -> Unit){
        val buyRequest=caffApi.buyCaffItem(param)
        runCallOnBackgroundThread(buyRequest,onSucces,onError)
    }

    fun commentCaffItem(id: Int, comment: CommentCreationModel, onSucces: (Int)->Unit, onError: (Throwable) -> Unit){
        val commentRequest=caffApi.commentCaffItem(id, comment)
        runCallOnBackgroundThread(commentRequest,onSucces,onError)
    }

    fun deleteCaffItem(id: Int, onSuccess: (Unit) -> Unit, onError: (Throwable) -> Unit){
        val deleteRequest=caffApi.deleteCaffItemByID(id)
        runCallOnBackgroundThread(deleteRequest,onSuccess,onError)
    }


}