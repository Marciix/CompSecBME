package com.example.caffwebshop.network

import com.example.caffwebshop.model.UserAuthenticateModel
import com.example.caffwebshop.model.UserLoginResponse
import com.example.caffwebshop.model.UserRegistrationModel
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.POST

interface AuthApi {
    companion object{
        const val ENDPOINT_URL="https://caffshop-api.nkelemen.hu/"
    }

    @POST("auth/login")
    fun login(@Body param: UserAuthenticateModel): Call<UserLoginResponse>

    @POST("auth/register")
    fun register(@Body param: UserRegistrationModel): Call<Int>
}