package com.example.caffwebshop.model

import com.google.gson.annotations.SerializedName

data class CaffItemCreation (
    @SerializedName("name")
    val name: String,
    @SerializedName("description")
    val description: String
)
