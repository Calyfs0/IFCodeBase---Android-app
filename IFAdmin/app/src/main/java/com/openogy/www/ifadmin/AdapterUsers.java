package com.openogy.www.ifadmin;

import android.app.Activity;
import android.app.Dialog;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

import java.io.Serializable;
import java.util.ArrayList;

public class AdapterUsers extends ArrayAdapter<Users> implements Serializable {
    private Activity activity;
    private ArrayList<Users> Users;
    private static LayoutInflater inflater = null;
    private Context context;
    private Dialog dialogForMakingBid;
    private Dialog dialogForReport;
    private String highestBid;
    private ViewHolder holder;

    public AdapterUsers(Activity activity, int textViewResourceId, ArrayList<Users> _allUsers, Context context) {
        super(activity, textViewResourceId, _allUsers);

        try {
            this.context = context;
            this.activity = activity;
            this.Users = _allUsers;
            dialogForMakingBid = new Dialog(context);
            dialogForReport = new Dialog(context);
            highestBid = "";

            inflater = (LayoutInflater) activity.getSystemService(Context.LAYOUT_INFLATER_SERVICE);

        } catch (Exception e) {

        }
    }

    public void addListItemToAdapter(ArrayList<Users> list) {
        //add list to current arraylist of data
        Users.addAll(list);

        //Notify UI
        this.notifyDataSetChanged();
    }

    public void updateResults() {

        //Triggers the list update
        this.notifyDataSetChanged();
    }

    public int getCount() {
        return Users.size();
    }

    public Users getItem(Users position) {
        return position;
    }

    public long getItemId(int position) {
        return position;
    }

    public static class ViewHolder {
        public TextView display_name;
        public TextView display_email;
        public TextView display_password;
        public TextView display_city;
        public TextView display_phonenumber;
    }

    public View getView(final int position, View convertView, ViewGroup parent) {
        View vi = convertView;

        try {
            if (convertView == null) {

                vi = inflater.inflate(R.layout.layout_singleuser, null);
                holder = new ViewHolder();

                holder.display_name = vi.findViewById(R.id.txtName);
                holder.display_email = vi.findViewById(R.id.txtEmail);
                holder.display_password = vi.findViewById(R.id.txtPassword);
                holder.display_city = vi.findViewById(R.id.txtCity);
                holder.display_phonenumber = vi.findViewById(R.id.txtPhoneNumber);


                vi.setTag(holder);
            } else {
                holder = (ViewHolder) vi.getTag();
            }
            holder.display_name.setText(Users.get(position).FullName);
            holder.display_email.setText(Users.get(position).Email);
            holder.display_password.setText(Users.get(position).Password);
            holder.display_city.setText(Users.get(position).City);
            holder.display_phonenumber.setText(Users.get(position).PhoneNumber);
        } catch (Exception e) {
            e.printStackTrace();

        }
        return vi;
    }


}
