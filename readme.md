# MailQ

A mail queue microservice for sending templated emails over SMTP with GRPC connection layer

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
  string templateKey = 3;
  map<string,string> mailData = 4;
  map<string,string> subjectData = 5;
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