import 'package:flutter/foundation.dart' show kIsWeb;

class HostDescription {

  final String scheme;
  final String host;

  HostDescription(this.scheme, this.host);
}

class Constants {

  static const String debugWebStartHost = 'localhost:5001';
  static const String debugAndroidStartHost = '10.0.2.2:5001';
  static HostDescription getHost() => HostDescription("https", debugWebStartHost);

  static HostDescription getDebugHost() {
    if (kIsWeb) {
      return HostDescription("http", Constants.debugWebStartHost);
    } else {
      return  HostDescription("http", Constants.debugAndroidStartHost);
    }
  }
}