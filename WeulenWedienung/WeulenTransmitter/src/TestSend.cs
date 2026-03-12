using var httpClient = new HttpClient();
await httpClient.PostAsync("http://192.168.178.36/on", new StringContent("", System.Text.Encoding.UTF8, "application/json"));