package com.example.caffwebshop.network

import android.os.Handler
import android.util.Log
import android.widget.Toast
import com.example.caffwebshop.model.*
import okhttp3.MultipartBody
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import javax.xml.datatype.DatatypeConstants.SECONDS
import okhttp3.OkHttpClient
import java.util.concurrent.TimeUnit


class CAFFInteractor {
    private val caffApi: CAFFApi

    init {
        val okHttpClient = OkHttpClient.Builder()
            .connectTimeout(1, TimeUnit.MINUTES)
            .readTimeout(30, TimeUnit.SECONDS)
            .writeTimeout(15, TimeUnit.SECONDS)
            .build()

        val retrofit= Retrofit.Builder()
            .baseUrl(CAFFApi.ENDPOINT_URL)
            .addConverterFactory(GsonConverterFactory.create())
            .client(okHttpClient)
            .build()

        this.caffApi=retrofit.create(CAFFApi::class.java)
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

    @JvmOverloads
    fun getCaffItems(token: String, withOwner :Boolean = false,onSuccess: (List<CaffItemPublic>?)->Unit, onError: (Throwable) -> Unit){

        val getRequest=caffApi.getCaffItems(token,withOwner)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    @JvmOverloads
    fun getCaffItemsByID(token: String, withOwner :Boolean = false, param: Int, onSuccess: (CaffItemPublic?)->Unit, onError: (Throwable) -> Unit){
        val getRequest=caffApi.getCaffItemsByID(token, param, withOwner)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    fun getCaffItemsByIDDownload(token: String, param: Int, onSuccess: (ResponseBody?)->Unit, onError: (Throwable) -> Unit){
        val getRequest=caffApi.getCaffItemsByIDDownload(token, param)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    fun getCaffItemsByIDPreview(token: String, param: Int, onSuccess: (Void?)->Unit, onError: (Throwable) -> Unit){
        val getRequest=caffApi.getCaffItemsByIDPreview(token, param)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    @JvmOverloads
    fun getCaffItemsSearch(token: String, param: String, withOwner :Boolean = false, onSuccess: (List<CaffItemPublic>?)->Unit, onError: (Throwable) -> Unit){
        val getRequest=caffApi.getCaffItemsSearch(token, param, withOwner)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    @JvmOverloads
    fun getCaffItemsByIDComment(token: String, param: Int, withAuthors :Boolean = false, onSuccess: (List<CommentPublic>?)->Unit, onError: (Throwable) -> Unit){
        val getRequest=caffApi.getCaffItemsByIDComment(token, param, withAuthors)
        runCallOnBackgroundThread(getRequest,onSuccess,onError)

    }

    fun uploadCaffItem(token: String, file:MultipartBody.Part, onSucces: (IdResult?)->Unit, onError: (Throwable) -> Unit){
        val uploadRequest=caffApi.uploadCaffItem(token, file)
        Log.i("body", uploadRequest.request().body().toString())
        Log.i("headers" ,uploadRequest.request().headers().toString())

        runCallOnBackgroundThread(uploadRequest,onSucces,onError)
    }

    fun buyCaffItem(token: String, param: Int, onSucces: (Void?)->Unit, onError: (Throwable) -> Unit){
        val buyRequest=caffApi.buyCaffItem(token, param)
        //Log.i("callrequest", buyRequest.request().toString())
       // Log.i("statuscode" ,buyRequest.execute().code().toString())
        runCallOnBackgroundThread(buyRequest,onSucces,onError)
    }

    fun commentCaffItem(token: String, id: Int, comment: CommentCreationModel, onSucces: (IdResult?)->Unit, onError: (Throwable) -> Unit){
        val commentRequest=caffApi.commentCaffItem(token, id, comment)
        runCallOnBackgroundThread(commentRequest,onSucces,onError)
    }

    fun deleteCaffItem(token: String, id: Int, onSuccess: (Void?) -> Unit, onError: (Throwable) -> Unit){
        val deleteRequest=caffApi.deleteCaffItemByID(token, id)
        runCallOnBackgroundThread(deleteRequest,onSuccess,onError)
    }




}