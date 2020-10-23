package com.example.caffwebshop.network

import retrofit2.Call
import retrofit2.http.DELETE
import retrofit2.http.PUT
import retrofit2.http.Path

interface UserApi {
    companion object{
        const val ENDPOINT_URL="https://caffshop-api.nkelemen.hu/"
    }

    @PUT("users/{id}")
    fun addUser(@Path("id") id: Int): Call<Unit>

    @DELETE("users/{id}")
    fun deleteUser(@Path("id") id: Int): Call<Unit>
}