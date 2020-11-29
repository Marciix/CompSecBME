package com.example.caffwebshop.network

import com.example.caffwebshop.model.UserModifyModel
import com.example.caffwebshop.model.UserPublic
import retrofit2.Call
import retrofit2.http.*

interface UserApi {
    companion object{
        const val ENDPOINT_URL="https://caffshop-api.nkelemen.hu/"
    }

    @GET("users")
    fun getUsers(@Header("Authorization") token: String): Call<List<UserPublic>?>

    @DELETE("users/{id}")
    fun deleteUser(@Header("Authorization") token:String, @Path("id") id: Int): Call<Void?>

    @PUT("users/{id}")
    fun modifyUser(@Header("Authorization") token:String, @Path("id") id: Int, @Body param: UserModifyModel): Call<Void?>
}