package com.openogy.www.ifadmin;

import android.app.Dialog;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.v4.widget.SwipeRefreshLayout;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.Window;
import android.widget.AbsListView;
import android.widget.AdapterView;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.lang.reflect.Type;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.concurrent.ExecutionException;

public class SearchUsers extends AppCompatActivity {

    private ListView listview;
    private ImageView iv;
    private GetAllUsersAsyncTask adUsers;
    public View footer;
    public Handler mHandler;
    public boolean isLoading = false;
    public AdapterUsers adbUsers;
    public int offset = 0;
    public int counter = 0;
    public int responseCode;
    View view;
    TextView tvNoAds;
    SwipeRefreshLayout pullToRefresh;
    int refreshcounter = 0;
    String userString;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_all_users);

        Intent intent = getIntent();
        userString = intent.getStringExtra("userString");

        String url ="http://api.ifadvertisings.com/api/User/AllUsersFromAdminSearch";
        new GetAllUsersAsyncTask().execute(url);

        tvNoAds = findViewById(R.id.tvNoAds);
        tvNoAds.setVisibility(View.GONE);

        listview = findViewById(R.id.UserListView);
        pullToRefresh = findViewById(R.id.pullToRefresh);

        pullToRefresh.setOnRefreshListener(new SwipeRefreshLayout.OnRefreshListener() {
            @Override
            public void onRefresh() {
                offset = 0;
                counter = 0;
                refreshcounter = 1;
                isLoading = false;
                //Resetting the categories and filter
                //SetDefaultMenuValues();
                String url = "http://api.ifadvertisings.com/api/User/AllUsersFromAdminSearch";
                new GetAllUsersAsyncTask().execute(url);
            }
        });

        pullToRefresh.setOnChildScrollUpCallback(new SwipeRefreshLayout.OnChildScrollUpCallback() {
            @Override
            public boolean canChildScrollUp(@NonNull SwipeRefreshLayout parent, @Nullable View child) {
                pullToRefresh.setRefreshing(false);
                int position = listview.getFirstVisiblePosition();
                if(position == 0)
                    return false;
                else
                    return true;
            }


        });

        footer = getLayoutInflater().inflate(R.layout.listview_footer,listview,false);

        mHandler = new MyHandler();

        listview.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> adapterView, View view, int i, long l) {

                Ads adControl = (Ads)adapterView.getItemAtPosition(i);
            }
        });


        listview.setOnScrollListener(new AbsListView.OnScrollListener() {
            @Override
            public void onScrollStateChanged(AbsListView absListView, int i) {

            }

            @Override
            public void onScroll(AbsListView absListView,  int firstVisibleItem,
                                 int visibleItemCount, int totalItemCount) {
                if(absListView.getLastVisiblePosition() == totalItemCount - 1 && listview.getCount() >= 7 && isLoading == false)
                {
                    isLoading = true;
                    offset = offset + 7;


                    String url = "http://api.ifadvertisings.com/api/User/AllUsersFromAdminSearch";
                    adUsers = (GetAllUsersAsyncTask) new GetAllUsersAsyncTask().execute(url);
                }
            }
        });

    }

    public class MyHandler extends Handler {
        @Override
        public void handleMessage(Message msg) {
            super.handleMessage(msg);
            switch (msg.what) {
                case 0:
                    //Add loading view during search processing
                    listview.addFooterView(footer,null,false);
                    break;
                case 1:
                    //Update data adapter and UI
                    adbUsers.addListItemToAdapter((ArrayList<Users>)msg.obj);
                    //Renove Footer after update
                    listview.removeFooterView(footer);
                    isLoading = false;
                    break;
                default:
                    break;
            }
        }
    }

    public class ThreadGetMoreData extends Thread {
        @Override
        public void run() {
            super.run();
            // mHandler.sendEmptyMessage(0);

            String adsList = null;
            try {

                adsList = adUsers.get();

                if(adsList != null)
                {
                    ArrayList<Users> arrayList = null;
                    Gson gson = new Gson();
                    Type collectionType = new TypeToken<Collection<Users>>() {
                    }.getType();
                    Collection<Users> adsCollection = gson.fromJson(adsList, collectionType);
                    Users[] adsArray = adsCollection.toArray(new Users[adsCollection.size()]);

                    arrayList = new ArrayList<Users>(Arrays.asList(adsArray));

                    Thread.sleep(2000);

                    Message msg = mHandler.obtainMessage(1, arrayList);
                    mHandler.sendMessage(msg);
                }



            } catch (InterruptedException e) {
                e.printStackTrace();
            } catch (ExecutionException e) {
                e.printStackTrace();
            }

        }
    }

    private void LoadingData(String adsList) throws InterruptedException, ExecutionException {
        ArrayList<Users> arrayList = null;
        if(adsList == null)
        {
            Toast.makeText(SearchUsers.this,"No Ads",Toast.LENGTH_SHORT).show();
            listview.setAdapter(null);
            tvNoAds.setVisibility(View.VISIBLE);
        }


        else {

            Gson gson = new Gson();
            Type collectionType = new TypeToken<Collection<Users>>() {
            }.getType();
            Collection<Users> adsCollection = gson.fromJson(adsList, collectionType);
            Users[] adsArray = adsCollection.toArray(new Users[adsCollection.size()]);

            arrayList = new ArrayList<Users>(Arrays.asList(adsArray));

            //then populate myListItems
            adbUsers = new AdapterUsers (SearchUsers.this, 0, arrayList, SearchUsers.this);
            if(adbUsers == null)
            {
                Toast.makeText(SearchUsers.this,"No Users",Toast.LENGTH_SHORT).show();
            }
            else
            {
                //listview.setDivider(null);
                listview.setAdapter(adbUsers);
            }

        }
    }

    public class GetAllUsersAsyncTask extends AsyncTask<String, Void, String> {

        private Dialog dialog = new Dialog(SearchUsers.this, android.R.style.Theme_Black);
        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            if(counter == 0 && refreshcounter == 0)
            {
                View view = LayoutInflater.from(SearchUsers.this).inflate(R.layout.layout_progressbar, null);
                dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
                dialog.setCancelable(false);
                dialog.getWindow().setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
                dialog.setContentView(view);
                dialog.show();
            }
            else if(refreshcounter == 0)
                listview.addFooterView(footer,null,false);

        }

        @Override
        protected String doInBackground(String... urls) {
            // params comes from the execute() call: params[0] is the url.
            try {
                try {
                    return HttpPost(urls[0]);
                } catch (JSONException e) {
                    e.printStackTrace();
                    return null;
                }
            } catch (IOException e) {
                return null;
            }
        }

        @Override
        protected void onPostExecute(String s) {
            super.onPostExecute(s);
            if(responseCode == 200)
            {
                if(counter == 0)
                {
                    try {
                        LoadingData(s);
                    } catch (InterruptedException e) {
                        e.printStackTrace();
                    } catch (ExecutionException e) {
                        e.printStackTrace();
                    }
                    counter++;
                    // If task execution completed

                }
                else if(s != null)
                {
                    Thread thread = new ThreadGetMoreData();
                    thread.start();
                    //listview.removeFooterView(footer);
                }
                else if(s == null)
                    listview.removeFooterView(footer);
            }
            else if(counter == 0)
                tvNoAds.setVisibility(View.VISIBLE);

            if(dialog.isShowing()){
                // Dismiss/hide the progress dialog
                dialog.dismiss();
            }
            pullToRefresh.setRefreshing(false);
            if(refreshcounter >= 1)
                refreshcounter = 0;
        }
    }

    private  String HttpPost(String myUrl) throws IOException, JSONException {

        URL url = new URL(myUrl);

        // 1. create HttpURLConnection
        HttpURLConnection conn = (HttpURLConnection) url.openConnection();
        conn.setRequestMethod("POST");
        conn.setRequestProperty("Content-Type", "application/json; charset=utf-8");

        // 2. build JSON object
        JSONObject jsonObject = buidJsonObject();

        // 3. add JSON content to POST request body
        setPostRequestContent(conn, jsonObject);

        // 4. make POST request to the given URL
        conn.connect();

        InputStreamReader inputStreamReader = new InputStreamReader(conn.getInputStream());
        BufferedReader bufferedReader = new BufferedReader(inputStreamReader);
        StringBuilder stringBuilder = new StringBuilder();
        String bufferedStrChunk = null;

        StringBuffer stringBuffer = new StringBuffer();
        String line;
        while ((line = bufferedReader.readLine()) != null)
        {
            stringBuilder.append(line);
        }

        responseCode = conn.getResponseCode();
        conn.disconnect();
        bufferedReader.close();
        inputStreamReader.close();


        // 5. return response message
        return stringBuilder.toString();

    }

    private  JSONObject buidJsonObject() throws JSONException {


        JSONObject jsonObject = new JSONObject();
        jsonObject.accumulate("Offset",offset);
        jsonObject.accumulate("userString",userString);
        return jsonObject;
    }

    private  void setPostRequestContent(HttpURLConnection conn,
                                        JSONObject jsonObject) throws IOException {

        OutputStream os = conn.getOutputStream();
        BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(os, "UTF-8"));
        writer.write(jsonObject.toString());
        Log.i(MainActivity.class.toString(), jsonObject.toString());
        writer.flush();
        writer.close();
        os.close();
    }
}
