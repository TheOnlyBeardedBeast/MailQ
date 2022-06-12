# MailQ

A mail queue microservice for sending templated emails over SMTP with GRPC connection layer

## Templates

MailQ uses MJML in combination with Mustache to render emails. MJML templates are prerendered and cached in memory. The subjects only use Mustache for templating.

Both the email and the subject accepts a Dictionary (list of key value pairs where both values are strings). You should use the same keys in your data as in you template.

The `.proto` contract describes the functionality enough, if you have any question feel free to create an issue. 

## ENV variables
```
- SMTP_PORT: required, represents the port number of the SMTP service
- SMTP_HOST: required, represents the SMTP host
- SMTP_EMAIL: required, represents the SMTP username and also used as the mailfrom
- SMTP_PASS: required, represents the SMTP password
- SMTP_TLS: optional, true or false, true if TLS enabled, default false
- API_KEY: required, represents the API key which secures the api access
- MAIL_INTERVAL: optional, represents the interval length in seconds for queing the emails, default 60 
```

## Run with docker

```bash
docker run -e SMTP_PORT=1025 -e SMTP_HOST=testmailhost.com -e SMTP_EMAIL=test@email.com -e SMTP_PASS=password -e API_KEY=API_KEY theonlybeardedbeast/mailq
```

## The API KEY should be included in the gRPC metadata as

```
X-API-KEY=yoursecretapikey
```

```proto
syntax = "proto3";

import "google/protobuf/timestamp.proto";

option csharp_namespace = "MailQ";

package mailcom;

service Mailcom {
  rpc SendMail (SendMailRequest) returns (SendMailResponse);
  rpc AddTemplate (AddTemplateRequest) returns (AddTemplateResponse);
  rpc UpdateTemplate (AddTemplateRequest) returns (AddTemplateResponse);
  rpc RemoveTemplate (RemoveTemplateRequest) returns (AddTemplateResponse);
}

message SendMailRequest {
  string to = 1;
  string templateKey = 2;
  map<string,string> mailData = 3;
  map<string,string> subjectData = 4;
}

message SendMailResponse {
  google.protobuf.Timestamp queued = 1;
  string cancellationToken = 2;
}

message AddTemplateRequest {
  string key = 1;
  string mailTemplate = 2;
  string subjectTemplate = 3;
}

message AddTemplateResponse {
  string key = 1;
}

message RemoveTemplateRequest {
  string key = 1;
}

message RemoveTemplateResponse {
  string key = 1;
}
```
## Example Loadtest

results simulates 100 paralel users for 1minute
```
checks...............: 100.00% ✓ 6000      ✗ 0
data_received........: 1.0 MB  17 kB/s
data_sent............: 1.3 MB  22 kB/s
grpc_req_duration....: avg=6.07ms min=1.97ms med=4.28ms max=35.74ms p(90)=11.58ms p(95)=16.41ms
iteration_duration...: avg=1s     min=1s     med=1s     max=1.04s   p(90)=1.01s   p(95)=1.02s
iterations...........: 6000    98.982539/s
vus..................: 100     min=100     max=100
vus_max..............: 100     min=100     max=100
```