package com.example.caffwebshop.network

import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class UserInteractor {
    private val userApi: UserApi

    init {
        val retrofit= Retrofit.Builder()
            .baseUrl(UserApi.ENDPOINT_URL)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
        this.userApi=retrofit.create(UserApi::class.java)
    }
}