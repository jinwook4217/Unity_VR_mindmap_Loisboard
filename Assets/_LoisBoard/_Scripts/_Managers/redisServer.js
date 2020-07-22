// var app = require('http').createServer(handler)
// var fs = require('fs');
// var redis = require('redis');
//     client = redis.createClient(6379,'127.0.0.1');

// app.listen(3000);

// function handler(req, res) {
//     fs.readFile(__dirname + '/index.html',
//         function (err, data) {
//             if (err) {
//                 res.writeHead(500);
//                 return res.end('Error loading index.html');
//             }

//             res.writeHead(200);
//             res.end(data);
//         });
// }


// // Start socket.io
// var io = require('socket.io')(app);

// io.on('connection', function (socket) {

//     socket.on('save', function (data) {
//         client.hmset(data.deviceId+"_"+data.mapId,["mapId",data.mapId,"title",data.title, "date",data.date,"themeId",data.themeId,"list",JSON.stringify(data.list)]);
//         client.zadd(data.deviceId,data.mapId,data.mapId);
//         //socket.emit('saveResult');
//     });
//     socket.on('load',function(data){
//         client.hgetall(data, function(err, reply) {
//             //reply is null when the key is missing 
//             var temp=JSON.parse(reply.list);
//             var dataSet={
//                 mapId:reply.mapId,
//                 themeId:reply.themeId,
//                 list:temp
//             }
//             socket.emit('loadReturn',dataSet);
//         });
//     });
//     socket.on("createRoom", function(data){
//       console.log(data);
//         socket.join('room');
//     });
//     socket.on("broadcast",function(data){
//       console.log("broadcasting...");
//       var nodedatasWrap={
//         nodedatas:data.list
//       }
//         socket.broadcast.in('room').emit('stateNow', nodedatasWrap);
//         //io.sockets.in('room').emit('stateNow', data);
//     });
//     socket.on("reload",function(data){
//       client.zrange(data,0,-1,function(err,reply){
//         var mapIds = {
//           Ids:reply
//         }
//         socket.emit("returnMapIds",mapIds);
//       });
//     })
//     socket.on("login",function(data){
//       client.zrange(data,0,-1,function(err,reply){
//         var mapIds = {
//           Ids:reply
//         }
//         socket.emit("returnMapIds",mapIds);
//       });
//     });
//     socket.on("searchMapData",function(data){
//       client.hmget(data,"date","title","mapId","themeId",function(err,reply){
//         var serverData={
//           mapId:reply[2],
//           title:reply[1],
//           date:reply[0],
//           themeId:reply[3]
//         }
//         socket.emit("returnMapDatas",serverData);
//       });
//     });

// });