package com.example.caffwebshop.model

import com.google.gson.annotations.SerializedName

data class UserPublic (
    @SerializedName("id")
    val id: Int,
    @SerializedName("userName")
    val userName: String,
    @SerializedName("email")
    val email: String,
    @SerializedName("firstName")
    val firstName: String,
    @SerializedName("lastName")
    val lastName: String

)