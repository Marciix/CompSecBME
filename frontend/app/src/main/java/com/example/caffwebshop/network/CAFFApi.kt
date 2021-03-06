package com.example.caffwebshop.network

import com.example.caffwebshop.model.*
import okhttp3.MultipartBody
import okhttp3.RequestBody
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.http.*


interface CAFFApi {
    companion object{
        const val ENDPOINT_URL="https://caffshop-api.nkelemen.hu/"
    }

    @GET("caffitems")
    fun getCaffItems(@Header("Authorization") token:String, @Query("withOwner") withOwner: Boolean): Call<List<CaffItemPublic>>

    @GET("caffitems/{id}")
    fun getCaffItemsByID(@Header("Authorization") token:String, @Path("id") id: Int, @Query("withOwner") withOwner: Boolean): Call<CaffItemPublic>

    @GET("caffitems/{id}/download")
    fun getCaffItemsByIDDownload(@Header("Authorization") token:String,@Path("id") id: Int): Call<ResponseBody>

    @GET("caffitems/{id}/preview.jpg")
    fun getCaffItemsByIDPreview(@Header("Authorization") token:String,@Path("id") id: Int): Call<Void>

    @GET("caffitems/search/{keyword}")
    fun getCaffItemsSearch(@Header("Authorization") token:String,@Path("keyword") keyword: String, @Query("withOwner") withOwner: Boolean): Call<List<CaffItemPublic>>

    @GET("caffitems/{id}/comments")
    fun getCaffItemsByIDComment(@Header("Authorization") token:String, @Path("id") id: Int, @Query("withAuthors") withOwner: Boolean): Call<List<CommentPublic>>

    @Multipart
    @POST("caffitems/upload")
    fun uploadCaffItem(@Header("Authorization") token:String, @Part file: MultipartBody.Part): Call<IdResult>

    @POST("caffitems/{id}/buy")
    fun buyCaffItem(@Header("Authorization") token:String,@Path("id") id: Int):Call<Void>

    @POST("caffitems/{id}/comments")
    fun commentCaffItem(@Header("Authorization") token:String,@Path("id") id: Int, @Body comment: CommentCreationModel): Call<IdResult>

    @DELETE("caffitems/{id}")
    fun deleteCaffItemByID(@Header("Authorization") token:String, @Path("id") id: Int): Call<Void>



}