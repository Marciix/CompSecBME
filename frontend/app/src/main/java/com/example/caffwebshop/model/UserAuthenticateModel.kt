package com.example.caffwebshop.model

import com.google.gson.annotations.SerializedName

data class UserAuthenticateModel (
    @SerializedName("username")
    val username: String,
    @SerializedName("password")
    val password:String
)