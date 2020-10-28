package com.example.caffwebshop.fragment

import android.app.Activity
import android.app.Dialog
import android.content.DialogInterface
import android.graphics.Color
import android.os.Bundle
import android.support.v4.app.DialogFragment
import android.support.v7.app.AlertDialog
import android.view.LayoutInflater
import android.view.View
import android.widget.TextView
import android.widget.Toast
import com.example.caffwebshop.R
import com.example.caffwebshop.model.CaffItemPublic
import com.example.caffwebshop.model.CommentPublic
import com.example.caffwebshop.network.CAFFInteractor
import java.lang.reflect.Array.set


class DescriptionDialogFragment: DialogFragment() {

    interface DescriptionDataListener {

        fun getToken():String
        fun getId():Int

    }

    private lateinit var listener:DescriptionDataListener
    private lateinit var a:Activity
    private lateinit var tvID: TextView
    private lateinit var tvName: TextView
    private lateinit var tvDescription: TextView
    private lateinit var tvOwnerid: TextView
    private lateinit var tvUploadedAt: TextView
    private lateinit var tvOwner: TextView
    private lateinit var viewAct: View
    private val caffInteractor= CAFFInteractor()
    private lateinit var description: CaffItemPublic

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        val act =this.activity
        a=this.activity as Activity

        if(act is DescriptionDialogFragment.DescriptionDataListener){
            listener=act
        }
        else{
            throw RuntimeException("Activity must implement SearchListener")
        }

    }

    override fun onStart() {
        super.onStart()
        (dialog as AlertDialog).getButton(AlertDialog.BUTTON_POSITIVE).setTextColor(Color.BLACK)
        (dialog as AlertDialog).getButton(AlertDialog.BUTTON_NEGATIVE).setTextColor(Color.BLACK)

    }

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        val builder = AlertDialog.Builder(activity!!)

        builder.setView(getContentView())
        builder.setTitle("Descriptions")
        builder.setPositiveButton("ok"
        ) { dialog, _ -> dialog.cancel() }
        return builder.create()

        return super.onCreateDialog(savedInstanceState)
    }



    private fun getContentView(): View {
        viewAct = LayoutInflater.from(context).inflate(R.layout.li_description, null)
        tvID=viewAct.findViewById(R.id.tv_caffID)
        tvName=viewAct.findViewById(R.id.tv_name)
        tvDescription=viewAct.findViewById(R.id.tv_description)
        tvOwner=viewAct.findViewById(R.id.tv_owner)
        tvUploadedAt=viewAct.findViewById(R.id.tv_uploadedAt)
        tvOwnerid=viewAct.findViewById(R.id.tv_ownerId)

        caffInteractor.getCaffItemsByID(listener.getToken(), withOwner = true, param = listener.getId(),
                onSuccess = this::onLoadDescriptionSuccess, onError = this::onLoadDescriptionsError )

        return viewAct
    }

    private fun onLoadDescriptionSuccess(caffItem: CaffItemPublic) {

        caffItem.id?.let { tvID.setText(caffItem.id.toString()) }
        caffItem.name?.let { tvName.setText(caffItem.name) }
        caffItem.uploadedAt?.let { tvUploadedAt.setText(caffItem.uploadedAt) }
        caffItem.description?.let { tvID.setText(caffItem.description) }
        caffItem.owner.userName?.let { tvOwner.setText(caffItem.owner.userName) }
        caffItem.ownerId?.let { tvOwnerid.setText(caffItem.ownerId.toString()) }





    }

    private fun onLoadDescriptionsError(e: Throwable) {
        e.printStackTrace()
        Toast.makeText(this.activity?.applicationContext, "Unable to load description!", Toast.LENGTH_LONG).show()
    }

}