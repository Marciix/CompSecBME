package com.example.caffwebshop.network

import com.example.caffwebshop.model.CaffItemCreation
import com.example.caffwebshop.model.CaffItemPublic
import com.example.caffwebshop.model.CommentCreationModel
import com.example.caffwebshop.model.CommentPublic
import retrofit2.Call
import retrofit2.http.*


interface CAFFApi {
    companion object{
        const val ENDPOINT_URL="https://caffshop-api.nkelemen.hu/"
    }

    @GET("caffitems")
    fun getCaffItems(@Header("Authorization") token:String): Call<List<CaffItemPublic>>

    @GET("caffitems/{id}")
    fun getCaffItemsByID(@Header("Authorization") token:String, @Path("id") id: Int): Call<CaffItemPublic>

    @GET("caffitems/{id}/download")
    fun getCaffItemsByIDDownload(@Header("Authorization") token:String,@Path("id") id: Int): Call<Void>

    @GET("caffitems/{id}/preview")
    fun getCaffItemsByIDPreview(@Header("Authorization") token:String,@Path("id") id: Int): Call<Void>

    @GET("caffitems/search/{keyword}")
    fun getCaffItemsSearch(@Header("Authorization") token:String,@Path("keyword") keyword: String): Call<CaffItemPublic>

    @GET("caffitems/{id}/comment")
    fun getCaffItemsByIDComment(@Header("Authorization") token:String, @Path("id") id: Int): Call<CommentPublic>

    @POST("caffitems/upload")
    fun uploadCaffItem(@Header("Authorization") token:String, @Body param: CaffItemCreation): Call<Int>

    @POST("caffitems/{id}/buy")
    fun buyCaffItem(@Header("Authorization") token:String,@Path("id") id: Int):Call<Void>

    @POST("caffitems/{id}/comments")
    fun commentCaffItem(@Header("Authorization") token:String,@Path("id") id: Int, @Body comment: CommentCreationModel): Call<Int>

    @DELETE("caffitems/{id}")
    fun deleteCaffItemByID(@Header("Authorization") token:String, @Path("id") id: Int): Call<Void>






}