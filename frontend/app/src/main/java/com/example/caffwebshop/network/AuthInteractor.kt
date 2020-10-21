package com.example.caffwebshop.network

import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class AuthInteractor {
    private val authApi: AuthApi

    init {
        val retrofit= Retrofit.Builder()
            .baseUrl(AuthApi.ENDPOINT_URL)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
        this.authApi=retrofit.create(AuthApi::class.java)
    }

}