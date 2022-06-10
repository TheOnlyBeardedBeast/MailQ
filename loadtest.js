import grpc from "k6/net/grpc";
import { check, sleep } from "k6";

const client = new grpc.Client();
client.load(["Protos"], "mailcom.proto");

export const options = {
  vus: 100,
  duration: "1m",
};

export default () => {
  client.connect("localhost:5259", {
    plaintext: true,
  });

  const mapdata = new Map();
  mapdata.set("ConfirmationToken", "token");

  const data = {
    to: "user@email.com",
    templateKey: "confirm",
  };
  const response = client.invoke("mailcom.Mailcom/SendMail", data, {
    metadata: {
      "X-API-KEY": "API_KEY",
    },
  });

  check(response, {
    "status is OK": (r) => r && r.status === grpc.StatusOK,
  });

  console.log(JSON.stringify(response.message));

  client.close();
  sleep(1);
};
