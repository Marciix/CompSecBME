package com.example.caffwebshop.network

import com.example.caffwebshop.model.UserModifyModel
import retrofit2.Call
import retrofit2.http.*

interface UserApi {
    companion object{
        const val ENDPOINT_URL="https://caffshop-api.nkelemen.hu/"
    }

    @PUT("users/{id}")
    fun addUser(@Header("Authorization") token:String, @Path("id") id: Int): Call<Void>

    @DELETE("users/{id}")
    fun deleteUser(@Header("Authorization") token:String, @Path("id") id: Int, @Body param: UserModifyModel): Call<Void>
}