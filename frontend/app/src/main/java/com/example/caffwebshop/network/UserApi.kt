package com.example.caffwebshop.network

import retrofit2.http.DELETE
import retrofit2.http.PUT
import retrofit2.http.Path

interface UserApi {
    companion object{
        const val ENDPOINT_URL="API_ENDPOINT"
    }

    @PUT("users/{id}")
    fun addUser(@Path("id") id: Int)

    @DELETE("users/{id}")
    fun deleteUser(@Path("id") id: Int)
}