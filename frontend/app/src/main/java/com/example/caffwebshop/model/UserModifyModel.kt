package com.example.caffwebshop.model

import com.google.gson.annotations.SerializedName

data class UserModifyModel (
    @SerializedName("firstName")
    val firstName: String,
    @SerializedName("lastName")
    val lastName: String

)