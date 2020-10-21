package com.example.caffwebshop.network

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
}