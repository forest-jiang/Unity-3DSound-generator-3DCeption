import socket
import sys
try:
  s = socket.socket()
  s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
  host = '127.0.0.1'
  port = 5555
  s.bind((host, port))
  s.listen(5)
  while True:
    c, addr = s.accept()
    total = 0
    totalReceive = ''
    while True:
      try:
        data = c.recv(1024)
        if data and len(data) == 1024:
          sys.stdout.write(':')
          total += len(data)
          totalReceive += data
        else:
          print '-breaking-', len(data)
          total += len(data)
          totalReceive += data
          break
      except socket.error, e:
        print "errer",e
        err = e.args[0]
        if err == errno.EAGAIN or err == errno.EWOULDBLOCK:
                print 'no more data'
        break
    print "One Image Received:", totalReceive[:10]+"   "+totalReceive[-10:], len(totalReceive)
    c.send("got")
#    c.send('Thank you for connecting, you send ' + str(total) + ' to us')
    c.close()                # Close the connection

finally:
  print "error, quit"
  s.close()
