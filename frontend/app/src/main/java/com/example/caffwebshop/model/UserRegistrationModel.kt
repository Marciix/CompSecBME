package com.example.caffwebshop.model

import com.google.gson.annotations.SerializedName

data class UserRegistrationModel (
    @SerializedName("userName")
    val userName:Int,
    @SerializedName("email")
    val email: String,
    @SerializedName("firstName")
    val firstName: String,
    @SerializedName("lastName")
    val lastName: String,
    @SerializedName("password")
    val password: String

)