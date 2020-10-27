package com.example.caffwebshop.adapter

import android.content.Context
import android.content.Intent
import android.content.Intent.FLAG_ACTIVITY_NEW_TASK
import android.net.Uri
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import com.bumptech.glide.Glide
import com.bumptech.glide.load.model.GlideUrl
import com.example.caffwebshop.R
import com.example.caffwebshop.activity.CommentsActivity
import com.example.caffwebshop.model.CaffItemPublic
import kotlinx.android.synthetic.main.content_comments.*
import kotlinx.android.synthetic.main.li_image.view.*
import java.net.URL

class ImagesAdapter(
    private val context: Context,
    private val caffItems: MutableList<CaffItemPublic>,
    private val token: String)
    : RecyclerView.Adapter<ImagesAdapter.ViewHolder>() {

    private val layoutInflater = LayoutInflater.from(context)

    init {
        this.caffItems.reverse()
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ImagesAdapter.ViewHolder {
        val view = layoutInflater.inflate(R.layout.li_image, parent, false)
        return ViewHolder(view)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val id=caffItems[position].id
        val url = "https://caffshop-api.nkelemen.hu/caffitems/$id/preview.jpg"
        val glideUrl = GlideUrl(url) { mapOf(Pair("Authorization", token)) }

        Glide.with(context)
            .load(glideUrl)
            .into(holder.imageView)

        holder.imageView.setOnClickListener {
            val intent=Intent(it.context, CommentsActivity::class.java)
            intent.flags = FLAG_ACTIVITY_NEW_TASK
            intent.putExtra("token", token)
            intent.putExtra("id", id)
            it.context.startActivity(intent)

        }
    }

    override fun getItemCount() = caffItems.size

    class ViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val imageView: ImageView = view.ivImage

    }

}