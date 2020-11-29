package com.example.caffwebshop.model

import com.google.gson.annotations.SerializedName

data class UserLoginResponse (
    @SerializedName("jwtToken")
    val jwtToken:String,
    @SerializedName("role")
    val role:String
)