package com.jlcvisitortracking;

import com.facebook.react.modules.network.OkHttpClientFactory;
import com.facebook.react.modules.network.OkHttpClientProvider;
import com.facebook.react.modules.network.ReactCookieJarContainer;
import okhttp3.OkHttpClient;
import okhttp3.OkHttpClient.Builder;
import java.io.InputStream;
import java.security.cert.CertificateFactory;
import java.security.cert.Certificate;
import java.security.cert.X509Certificate;
import java.security.KeyStore;
import javax.net.ssl.TrustManagerFactory;
import javax.net.ssl.SSLContext;
import javax.net.ssl.HttpsURLConnection;
import android.content.res.Resources;
import javax.net.ssl.SSLSocketFactory;
import java.io.BufferedInputStream;

public class CustomClientFactory implements OkHttpClientFactory {
    private Resources resources;

    CustomClientFactory(Resources resources) {
        this.resources = resources;
    }

    @Override
    public OkHttpClient createNewNetworkModuleClient() {
        OkHttpClient.Builder client = new OkHttpClient.Builder().cookieJar(new ReactCookieJarContainer());
        SSLSocketFactory sslSocketFactory = createSocketFactory(this.resources);
        if(sslSocketFactory != null) {
            client.sslSocketFactory(sslSocketFactory);
        }
        return OkHttpClientProvider.enableTls12OnPreLollipop(client).build();
    }

    private SSLSocketFactory createSocketFactory(Resources resources) {
        InputStream caInput = null;
        Certificate ca = null;
        try {
            CertificateFactory cf = CertificateFactory.getInstance("X.509");
            InputStream inputStream = resources.openRawResource(R.raw.ca_public_key);
            if(inputStream == null)
                System.out.println("input stream is null");
            else
                System.out.println("input stream is NOT null :-)");
            caInput = new BufferedInputStream(inputStream);
            ca = cf.generateCertificate(caInput);
            System.out.println("ca=" + ((X509Certificate) ca).getSubjectDN());
            String keyStoreType = KeyStore.getDefaultType();
            KeyStore keyStore = KeyStore.getInstance(keyStoreType);
            keyStore.load(null, null);
            keyStore.setCertificateEntry("ca", ca);
            caInput.close();
            String tmfAlgorithm = TrustManagerFactory.getDefaultAlgorithm();
            TrustManagerFactory tmf = TrustManagerFactory.getInstance(tmfAlgorithm);
            tmf.init(keyStore);
            SSLContext context = SSLContext.getInstance("TLS");
            context.init(null, tmf.getTrustManagers(), null);
            return context.getSocketFactory();
        } catch (Exception e) {
            e.printStackTrace();
        }
        return null;
    }
}