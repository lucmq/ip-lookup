# IP-Lookup
[![DeepSource](https://app.deepsource.com/gh/lucmq/ip-lookup.svg/?label=code+coverage&show_trend=false&token=XPRjAR9Vp0meiSpIB7tvVj-E)](https://app.deepsource.com/gh/lucmq/ip-lookup/)
[![DeepSource](https://app.deepsource.com/gh/lucmq/ip-lookup.svg/?label=active+issues&show_trend=false&token=XPRjAR9Vp0meiSpIB7tvVj-E)](https://app.deepsource.com/gh/lucmq/ip-lookup/)

A lightweight, fast, and efficient **in-memory** index for public IP location data,
designed for seamless integration with Docker.

The service can index the entire global IP network in about 200MB of memory, including
the overhead of the ASP.NET server.

## Deployment
```sh
docker run --name lookup-ip -d lucascm14/ip-lookup:latest
```

## API Documentation

### Endpoint: Lookup an IP Address
#### URL
`GET /lookup/{ip_address}`

#### Description
Looks up the information for the given IP address.

#### Response
- **Status Code:** 200 OK
- **Content:**
```json
{
  "city": "South Brisbane",
  "region": "Queensland",
  "country": "AU",
  "latitude": -27.4767,
  "longitude": 153.017
}
```

### Endpoint: Service Stats
#### URL
`GET /`

#### Description
Returns general information and statistics for the application.

#### Response
- **Status Code:** 200 OK
- **Content:**
```json
{
  "status": "SERVING",
  "index": {
    "ip_ranges": 3261621,
    "distinct_info_count": 388806
  },
  "import_task": {
    "start_time": "2024-06-26T21:01:12.2249912Z",
    "duration": "00:00:17.8363821"
  }
}
```

## Configuration - Environment Variables

| Environment Variable    | Description                                             |
|-------------------------|---------------------------------------------------------|
| `ImportTask__FileUri`   | Sets the URI to the DBIP CSV file with IP location data |

## Future Improvements
- **IPv6 Support**: Add support for IPv6 as a configurable add-on to the IPv4 index.
- **Scheduled Updates**: Implement a feature to check for DBIP updates on a scheduled
 basis. Currently, the index is updated when the host restarts.
- **Local Archive**: Use a local archive if it exists and is up-to-date to avoid
 unnecessary downloads and reduce startup time.
